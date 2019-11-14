FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.9 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS build
WORKDIR /src

COPY ["VultPay.PayService/VultPay.PayService.csproj", "VultPay.PayService/"]
RUN dotnet restore "VultPay.PayService/VultPay.PayService.csproj"

COPY . .
WORKDIR "/src/VultPay.PayService"

RUN dotnet build "VultPay.PayService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "VultPay.PayService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

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