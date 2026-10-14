using MyLab.ApiClient.JsonSerialization;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Problems;
using Xunit;

namespace UnitTests.Problems;

public class ProblemDetailsBehavior
{
    static readonly MicrosoftJsonSerializer StjSerializer = new(new ApiJsonSettings());

    const string ProblemDetailsJson = """
                                      {
                                        "type": "https://example.org/probs/out-of-credit",
                                        "title": "You do not have enough credit.",
                                        "status": 403,
                                        "detail": "Your current balance is 30, but that costs 50.",
                                        "instance": "/account/12345/msgs/abc",
                                        "balance": 30,
                                        "accounts": ["a", "b"]
                                      }
                                      """;

    const string ValidationProblemDetailsJson = """
                                                {
                                                  "title": "One or more validation errors occurred.",
                                                  "status": 400,
                                                  "errors": {
                                                    "Name": ["Required"]
                                                  },
                                                  "traceId": "00-abc"
                                                }
                                                """;

    [Fact]
    public void ShouldDeserializeExtensionMembersWithNewtonsoft()
    {
        //Act
        var problem = NewtonJsonSerializer.Default.Deserialize<ProblemDetails>(ProblemDetailsJson);

        //Assert
        AssertProblemDetailsWithExtensions(problem);
    }

    [Fact]
    public void ShouldDeserializeExtensionMembersWithStj()
    {
        //Act
        var problem = StjSerializer.Deserialize<ProblemDetails>(ProblemDetailsJson);

        //Assert
        AssertProblemDetailsWithExtensions(problem);
    }

    [Fact]
    public void ShouldDeserializeValidationErrorsWithNewtonsoft()
    {
        //Act
        var problem = NewtonJsonSerializer.Default.Deserialize<ValidationProblemDetails>(ValidationProblemDetailsJson);

        //Assert
        AssertValidationProblemDetails(problem);
    }

    [Fact]
    public void ShouldDeserializeValidationErrorsWithStj()
    {
        //Act
        var problem = StjSerializer.Deserialize<ValidationProblemDetails>(ValidationProblemDetailsJson);

        //Assert
        AssertValidationProblemDetails(problem);
    }

    static void AssertProblemDetailsWithExtensions(ProblemDetails problem)
    {
        Assert.Equal("https://example.org/probs/out-of-credit", problem.Type);
        Assert.Equal("You do not have enough credit.", problem.Title);
        Assert.Equal(403, problem.Status);
        Assert.Equal(2, problem.Extensions.Count);
        Assert.True(problem.Extensions.ContainsKey("balance"));
        Assert.True(problem.Extensions.ContainsKey("accounts"));
    }

    static void AssertValidationProblemDetails(ValidationProblemDetails problem)
    {
        Assert.Equal(400, problem.Status);
        Assert.True(problem.Errors.ContainsKey("Name"));
        Assert.Equal(["Required"], problem.Errors["Name"]);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
    }
}
