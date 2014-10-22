﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Rest.Tests.TestRestServer;
using Should;
using Xunit;

namespace Rest.Tests
{
    public class Tests
    {
        [Fact]
        public async Task Should_get_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos");

                foos.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public async Task Should_get_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

                var foos = await restClient.GetAsync<List<Foo>>("/api/foos");

                foos.Count.ShouldEqual(3);
            }
        }

        [Fact]
        public void Should_handle_error_when_getting_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                const string ErrorMessage = "error";
                
                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<List<Foo>>("/api/error/" + ErrorMessage).Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.ApiError.Message.ShouldEqual(ErrorMessage);
            }
        }

        [Fact]
        public void Should_handle_error_when_getting_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");
                const string ErrorMessage = "error";

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<List<Foo>>("/api/error/" + ErrorMessage).Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.ApiError.Message.ShouldEqual(ErrorMessage);
            }
        }

        [Fact]
        public void Should_handle_not_found_when_getting_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<List<Foo>>("/api/unknown").Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.ApiError.HttpStatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void Should_handle_not_found_when_getting_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<List<Foo>>("/api/unknown").Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.ApiError.HttpStatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        //[Fact]
        //public async Task Should_get_error()
        //{
        //    using (var server = TestServer.Create<Startup>())
        //    {
        //        var restClient = new RestClient(server.HttpClient);
        //        restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

        //        var foos = await restClient.GetAsync<List<Foo>>("/api/error/error");

        //    }
        //}
    }


}