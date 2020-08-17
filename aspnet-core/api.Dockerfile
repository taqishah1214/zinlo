FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

COPY ./Zinlo.Web.sln ./

COPY common.props ./

COPY ./src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

COPY ./test/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p test/${file%.*}/ && mv $file test/${file%.*}/; done

RUN dotnet clean 
RUN dotnet restore

COPY ./src/. ./src
COPY ./test/. ./test

WORKDIR /source/src/Zinlo.Web.Host
RUN dotnet build "Zinlo.Web.Host.csproj" -c release -o /source/build

FROM build AS publish
RUN dotnet publish -c release -o /source/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-buster-slim
WORKDIR /app
COPY --from=publish /source/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Zinlo.Web.Host.dll"]


