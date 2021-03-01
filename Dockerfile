FROM harbor.int/mcr.microsoft.com/dotnet/runtime:5.0-alpine AS runtime

RUN echo "http://artifactory.ru/artifactory/alpine/v3.11/main/" > /etc/apk/repositories
RUN apk update && apk add ca-certificates tzdata && rm -rf /var/cache/apk/*
RUN update-ca-certificates

# Setting up timezone
RUN cp /usr/share/zoneinfo/Europe/Moscow /etc/localtime && echo "Europe/Moscow" > /etc/timezone && apk del tzdata

WORKDIR /app
COPY . ./
ENV ASPNETCORE_ENVIRONMENT="Production"
ENTRYPOINT update-ca-certificates && dotnet ReleaseBoard.Web.dll
