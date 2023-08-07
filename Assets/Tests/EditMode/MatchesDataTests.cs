using NUnit.Framework;
using SimpleJSON;

public class MatchesDataTests
{
    [Test]
    public void FillMatchesData_Test()
    {
        // Arrange
        MatchesData matchesData = new MatchesData();
        JSONNode testJSONData = JSON.Parse(@"[
            {
                ""createdAt"": ""2023-04-30T07:38:53.660Z"",
                ""standings"": [""Luigi"", ""Mario"", ""Bowser"", ""Peach""]
            },
            {
                ""createdAt"": ""2023-05-01T07:38:53.660Z"",
                ""standings"": [""Mario"", ""Yoshi"", ""Toad"", ""Luigi""]
            }
        ]");

        // Act
        matchesData.FillMatchesData(testJSONData);

        // Assert
        Assert.AreEqual(2, matchesData.matches.Count);

        Match firstMatch = matchesData.matches[0];
        Assert.AreEqual("2023-04-30T07:38:53.660Z", firstMatch.createdAt);
        Assert.AreEqual(4, firstMatch.standings.Length);
        Assert.AreEqual("Luigi", firstMatch.standings[0]);
        Assert.AreEqual("Mario", firstMatch.standings[1]);
        Assert.AreEqual("Bowser", firstMatch.standings[2]);
        Assert.AreEqual("Peach", firstMatch.standings[3]);

        Match secondMatch = matchesData.matches[1];
        Assert.AreEqual("2023-05-01T07:38:53.660Z", secondMatch.createdAt);
        Assert.AreEqual(4, secondMatch.standings.Length);
        Assert.AreEqual("Mario", secondMatch.standings[0]);
        Assert.AreEqual("Yoshi", secondMatch.standings[1]);
        Assert.AreEqual("Toad", secondMatch.standings[2]);
        Assert.AreEqual("Luigi", secondMatch.standings[3]);
    }
}
