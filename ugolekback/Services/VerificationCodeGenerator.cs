namespace ugolekback.Services;

public interface IVerificationCodeGenerator {
    string GetCode();
}

public class VerificationCodeGenerator : IVerificationCodeGenerator {
    public string GetCode() {
        var newCode = Random.Shared.Next(1010, 9090);

        return Convert.ToString(newCode);
    }
}