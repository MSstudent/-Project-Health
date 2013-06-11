using Alexis.WindowsPhone.Social;

namespace Health
{
    public class Constants
    {
        public const string SHARE_IMAGE = "kiss";

        public static ClientInfo GetClient(SocialType type)
        {
            ClientInfo client = new ClientInfo();
            switch (type)
            {
                case SocialType.Weibo:
                    client.ClientId = "2730107943";
                    client.ClientSecret = "702b05f502837bfb48070ad222472f2f";
                    client.RedirectUri = "https://github.com/DelightRun/PhoneApp1";//if not set,left this property empty
                    break;
                case SocialType.Tencent:
                    client.ClientId = "";
                    client.ClientSecret = "";
                    break;
                case SocialType.Renren:
                    client.ClientId = "";
                    client.ClientSecret = "";
                    break;
                default:
                    break;
            }
            return client;
        }
    }
}
