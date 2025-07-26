# Используем официальный образ SDK .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore YT_Downloader.sln
RUN dotnet build YT_Downloader.sln -c Release -o /app/build

# Публикуем приложение
FROM build AS publish
RUN dotnet publish YT_Downloader.sln -c Release -o /app/publish

# Финальный рантайм образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 🔧 Установка yt-dlp и ffmpeg
RUN apt-get update && \
    apt-get install -y ffmpeg wget ca-certificates && \
    wget https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -O /usr/local/bin/yt-dlp && \
    chmod a+rx /usr/local/bin/yt-dlp && \
    rm -rf /var/lib/apt/lists/*

# Директория для данных
RUN mkdir -p /app/data && chmod 777 /app/data

COPY --from=publish /app/publish .

EXPOSE 5020/tcp

ENTRYPOINT ["dotnet", "YT_Downloader.dll"]
