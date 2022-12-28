添加文件夹授权方式

```csharp
...
var minio = new MinioClient()
    .WithEndpoint("192.168.21.109:9000")
    .WithCredentials("minioadmin",
    "minioadmin")
    .WithSSL(false)
    .Build();
//这里的uri参数和下面测试访问的地址保持一致，否则鉴权不通过
string url = minio.PresignedGetFolderPathAsync("http://127.0.0.1:9000",new PresignedGetFolderPathArgs()
    .WithBucket("test")
    .WithFolderPath("/")
    .WithExpiry(10_000))
    .Result;
...
```

## 官方项目地址：https://github.com/minio/minio-dotnet
