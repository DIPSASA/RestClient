﻿using System;
using System.Collections.Generic;
using System.Linq;
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

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/error/" + ErrorMessage).Result);

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

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/error/" + ErrorMessage).Result);

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

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/unknown").Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void Should_handle_not_found_when_getting_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");

                var aggregateException = Assert.Throws<AggregateException>(() => restClient.GetAsync<object>("/api/unknown").Result);

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void Should_throw_exception_for_unkown_server_mediatype()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/foos");

                var aggregateException = Assert.Throws<AggregateException>(() =>
                {
                    var result = restClient.GetAsync<List<Foo>>("/api/foos").Result;
                    return result;
                });

                aggregateException.InnerException.ShouldBeType<ApiException>();
                var apiException = (ApiException)aggregateException.InnerException;
                apiException.HttpStatusCode.ShouldEqual(HttpStatusCode.NotAcceptable);
            }
        }

        [Fact]
        public void Should_throw_exception_for_unkown_client_mediatype()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);
                restClient.HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                restClient.MediaTypeSerializers.Remove(restClient.MediaTypeSerializers.First(x => x is JsonMediaTypeSerializer));

                var aggregateException = Assert.Throws<AggregateException>(() =>
                {
                    var result = restClient.GetAsync<List<Foo>>("/api/foos").Result;
                    return result;
                });

                aggregateException.InnerException.ShouldBeType<NotSupportedException>();
            }
        }

        [Fact]
        public async Task Should_get_with_parameters()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var parameters = new Dictionary<string, string> { { "param1", "value1" }, { "param2", "value2" } };
                var foo = await restClient.GetAsync<Foo>("/api/foos", parameters);

                foo.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task Should_get_with_id()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var foo = await restClient.GetAsync<Foo>("/api/foos/{0}".FormatUri(1));

                foo.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task Should_post_json()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var id = await restClient.PostAsync<int>("/api/foos", new Foo(), "application/json");

                id.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task Should_post_xml()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var restClient = new RestClient(server.HttpClient);

                var id = await restClient.PostAsync<int>("/api/foos", new Foo(), "application/xml");

                id.ShouldEqual(1);
            }
        }
    }


}