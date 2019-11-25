FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine3.10 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine3.10 AS build
WORKDIR /src
COPY ["VultPay.PayService/VultPay.PayService.csproj", "VultPay.PayService/"]
COPY ["VultPay.Application/VultPay.Application.csproj", "VultPay.Application/"]
COPY ["VultPay.Domain/VultPay.Domain.csproj", "VultPay.Domain/"]
COPY ["VultPay.Infra/VultPay.Infra.csproj", "VultPay.Infra/"]
COPY ["VultPay.DataBase/VultPay.DataBase.csproj", "VultPay.DataBase/"]
RUN dotnet restore "VultPay.PayService/VultPay.PayService.csproj"
COPY . .
WORKDIR "/src/VultPay.PayService"
RUN dotnet build "VultPay.PayService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VultPay.PayService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VultPay.PayService.dll"]

# Installs latest packages.
RUN apk update \
		&& apk upgrade \
		&& apk add --no-cache yarn ca-certificates
#
COPY ["VultPay.PayService/ca-certificates/vultpay_ca_root.crt", "/etc/ssl/certs/"]

#
RUN cat /etc/ssl/certs/vultpay_ca_root.crt >> /usr/local/share/ca-certificates/ca-certificates.crt \
	&& update-ca-certificates
#
ENTRYPOINT ["dotnet", "VultPay.PayService.dll"]