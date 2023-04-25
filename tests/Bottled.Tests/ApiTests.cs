using Bottled.Api.Dtos;
using Bottled.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace Bottled.Tests;

public class ApiTests : IntegrationTestBase
{
    public ApiTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        DbContext.Messages.RemoveRange(DbContext.Messages);
        DbContext.SaveChanges();
    }

    [Fact]
    public async Task GetRandomMessage_WhenNoMessage_ShouldReturn404NotFound()
    {
        var response = await Client.GetAsync("/api/");

        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRandomMessage_WhenMessage_ShouldReturn200Ok()
    {
        var message = new Message() { Author = "T-1000", Content = "I'll be back." };

        await DbContext.AddAsync(message);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("/api/");

        var jsonString = await response.Content.ReadAsStringAsync();
        var messageDto = JsonConvert.DeserializeObject<MessageDto>(jsonString);

        response.Should().HaveStatusCode(HttpStatusCode.OK);
        messageDto?.Author.Should().Be(message.Author);
        messageDto?.Content.Should().Be(message.Content);
    }

    [Fact]
    public async Task WriteMessage_ShouldReturn200Ok()
    {
        var messageDto = new MessageDto()
        {
            Author = "T-1000",
            Content = "I'll be back."
        };

        var response = await Client.PostAsJsonAsync("/api/write", messageDto);

        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var message = DbContext.Messages.First();

        message.Author.Should().Be(messageDto.Author);
        message.Content.Should().Be(messageDto.Content);
    }
}