using Efatura.Shared.Contracts;
using Xunit;

namespace SmokeTests;

public class MesajKontratTesti
{
    [Fact]
    public void EfaturaMesaj_MessageId_OtomatikUretilir()
    {
        var mesaj = new EfaturaMesaj();

        Assert.NotEqual(Guid.Empty, mesaj.MessageId);
    }

    [Fact]
    public void EfaturaMesaj_IkiOrnek_MessageId_FarkliOlur()
    {
        var birinci = new EfaturaMesaj();
        var ikinci = new EfaturaMesaj();

        Assert.NotEqual(birinci.MessageId, ikinci.MessageId);
    }

    [Fact]
    public void EfaturaMesaj_OlusturmaZamani_SimdiyeYakin()
    {
        var mesaj = new EfaturaMesaj();

        Assert.True(Math.Abs((DateTimeOffset.UtcNow - mesaj.OlusturmaZamani).TotalSeconds) < 5);
    }
}
