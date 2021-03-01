using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using MediatR;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.SignalREvents.Server;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.ReadModels.Projections.Notification
{
    /// <summary>
    /// Отправляет уведомление о наборе изменений сущности в SignalR.
    /// </summary>
    public static class SendDiffEntitiesNotifications
    {
        /// <summary>
        /// Модель уведомление о том, что какой объект на что изменился.
        /// </summary>
        public sealed record Notification(string Type, Entity OldEntity, Entity NewEntity, IEnumerable<User> Users) : INotification;

        /// <summary>
        /// Обработчик.
        /// </summary>
        public class Handler : INotificationHandler<Notification>
        {
            private readonly CompareLogic compareLogic;
            private readonly IMediator mediator;
            
            /// <summary>
            /// Конструктор.
            /// </summary>
            /// <param name="mediator"><see cref="IMediator"/>.</param>
            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
                compareLogic = new CompareLogic
                {
                    Config =
                    {
                        UseHashCodeIdentifier = true
                    }
                };
            }
            
            /// <inheritdoc />
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                (string type, Entity oldEntity, Entity newEntity, IEnumerable<User> users) = notification;

                Validate(oldEntity, newEntity);
                
                ExpandoObject changesObject = GetChangedProperties(oldEntity, newEntity, nameof(Entity.Id));
                changesObject.TryAdd(nameof(Entity.Id), GetNotNullObject(oldEntity, newEntity).Id);

                string[] userIds = users?.Select(x => x.Sid).ToArray() ?? Array.Empty<string>();
                await mediator.Publish(new ServerSignalREvent(type, changesObject, userIds), cancellationToken);
            }

            private void Validate<T>(T oldObject, T newObject)
            {
                if (oldObject == null && newObject == null)
                {
                    throw new ArgumentNullException($"{nameof(newObject)}, {nameof(oldObject)}");
                }

                if (AreNotNull(oldObject, newObject) && oldObject.GetType() != newObject.GetType())
                {
                    throw new ArgumentException("Типы объектов должны совподать");
                }
            }

            private ExpandoObject GetChangedProperties<T>(T oldObject, T newObject, params string[] ignoreProperties)
            {
                ExpandoObject expandoObject = new ExpandoObject();
                
                IEnumerable<PropertyInfo> properties =
                    GetNotNullObject(oldObject, newObject)
                        .GetType()
                        .GetProperties()
                        .Where(x => !ignoreProperties.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
                
                foreach (PropertyInfo property in properties)
                {
                    if (AreNotNull(oldObject, newObject) && AreEqual(oldObject, newObject, property))
                    {
                        continue;
                    }

                    T obj = GetNotNullObject(oldObject, newObject);
                    expandoObject.TryAdd(property.Name, property.GetValue(obj, null));
                }

                return expandoObject;
            }

            private bool AreEqual<T>(T oldObject, T newObject, PropertyInfo property)
            {
                object oldObjectValue = property.GetValue(oldObject, null);
                object newObjectValue = property.GetValue(newObject, null);

                ComparisonResult comparisonResult =
                    compareLogic.Compare(oldObjectValue, newObjectValue);

                return comparisonResult.AreEqual;
            }

            private bool AreNotNull<T>(T oldObject, T newObject) => oldObject != null && newObject != null;

            private T GetNotNullObject<T>(T oldObject, T newObject) => newObject ?? oldObject;
        }
    }
}
