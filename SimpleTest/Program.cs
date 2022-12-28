/*
 * MinIO .NET Library for Amazon S3 Compatible Cloud Storage, (C) 2017 MinIO, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Net;
using System.Threading.Tasks;
using Minio;

namespace SimpleTest;

public class Program
{
    private static  void Main(string[] args)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                               | SecurityProtocolType.Tls11
                                               | SecurityProtocolType.Tls12;

        /// Note: s3 AccessKey and SecretKey needs to be added in App.config file
        /// See instructions in README.md on running examples for more information.
        var minio = new MinioClient()
            .WithEndpoint("192.168.21.109:9000")
            .WithCredentials("minioadmin",
                "minioadmin")
            .WithSSL(false)
            .Build();

        string url = minio.PresignedGetFolderPathAsync("http://127.0.0.1:9000",new PresignedGetFolderPathArgs()
            .WithBucket("test")
            .WithFolderPath("test2/")
            .WithExpiry(10_000))
            .Result;

        var getListBucketsTask = minio.ListBucketsAsync();

        try
        {
            Task.WaitAll(getListBucketsTask); // block while the task completes
        }
        catch (AggregateException aggEx)
        {
            aggEx.Handle(HandleBatchExceptions);
        }

        var list = getListBucketsTask.Result;
        foreach (var bucket in list.Buckets) Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);

        //Supply a new bucket name
        var bucketName = "mynewbucket";
        if (isBucketExists(minio, bucketName))
        {
            var remBuckArgs = new RemoveBucketArgs()
                .WithBucket(bucketName);
            var removeBucketTask = minio.RemoveBucketAsync(remBuckArgs);
            Task.WaitAll(removeBucketTask);
        }

        var mkBktArgs = new MakeBucketArgs()
            .WithBucket(bucketName);
        Task.WaitAll(minio.MakeBucketAsync(mkBktArgs));

        var found = isBucketExists(minio, bucketName);
        Console.WriteLine("Bucket exists? = " + found);
        Console.ReadLine();
    }

    private static bool isBucketExists(IMinioClient minio,
        string bucketName)
    {
        var bktExistsArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        var bucketExistTask = minio.BucketExistsAsync(bktExistsArgs);
        Task.WaitAll(bucketExistTask);
        return bucketExistTask.Result;
    }

    private static bool HandleBatchExceptions(Exception exceptionToHandle)
    {
        if (exceptionToHandle is ArgumentNullException)
        {
            //I'm handling the ArgumentNullException.
            Console.WriteLine("Handling the ArgumentNullException.");
            //I handled this Exception, return true.
            return true;
        }

        //I'm only handling ArgumentNullExceptions.
        Console.WriteLine("I'm not handling the {0}.", exceptionToHandle.GetType());
        //I didn't handle this Exception, return false.
        return false;
    }
}