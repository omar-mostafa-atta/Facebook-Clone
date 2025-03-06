using FacebookClone.Core.IRepository;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;


namespace FacebookClone.EF.Repository
{
	public class RedisOTPService
	{
		private readonly ConnectionMultiplexer _redis;
		private readonly IDatabase _db;
		private readonly int _otpExpirySeconds;

		public RedisOTPService(IConfiguration configuration)
		{
			_redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
			_db = _redis.GetDatabase();
			_otpExpirySeconds = int.Parse(configuration["Redis:OTPExpirySeconds"]);
		}
		public async Task StoreOTPAsync(string email, string otp)
		{
			await _db.StringSetAsync(email, otp, TimeSpan.FromSeconds(_otpExpirySeconds));
		}
		public async Task<string> GetOTPAsync(string email)
		{
			return await _db.StringGetAsync(email);
		}

		public async Task<bool> VerifyOTPAsync(string email, string inputOtp)
		{
			var storedOtp = await GetOTPAsync(email);
			if (storedOtp == inputOtp)
			{
				await _db.KeyDeleteAsync(email); // lw tl3 sa hms70 5las w hrg3 true
				return true;
			}
			return false;
		}
	}
}
