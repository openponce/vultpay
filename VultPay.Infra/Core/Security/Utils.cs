using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VultPay.Infra.Core.Security
{

    public enum KeySizes
    {
        F128 = 128,
        F256 = 256,
        F512 = 512,
        F1024 = 1024,
        F2048 = 2048,
        F4096 = 4096
    }

    public class Constants
    {
        public const int NUM_ITERATOR = 1000;
        public const KeySizes PGP_KEYSIZE = KeySizes.F512;
        public const KeySizes RSA_KEYSIZE = KeySizes.F512;
        public const long USERS_ORG_REDIS_DB = 1;
    }

    public sealed class BCEngines
    {
        static AesEngine _AesEngine;
        public static AesEngine AesEngine
        {
            get
            {
                if (_AesEngine == null)
                    _AesEngine = new AesEngine();

                return _AesEngine;
            }
        }

        static AesLightEngine _AesLightEngine;
        public static AesLightEngine AesLightEngine
        {
            get
            {
                if (_AesLightEngine == null)
                    _AesLightEngine = new AesLightEngine();

                return _AesLightEngine;
            }
        }

        static BlowfishEngine _BlowfishEngine;
        public static BlowfishEngine BlowfishEngine
        {
            get
            {
                if (_BlowfishEngine == null)
                    _BlowfishEngine = new BlowfishEngine();

                return _BlowfishEngine;
            }
        }

        static CamelliaEngine _CamelliaEngine;
        public static CamelliaEngine CamelliaEngine
        {
            get
            {
                if (_CamelliaEngine == null)
                    _CamelliaEngine = new CamelliaEngine();

                return _CamelliaEngine;
            }
        }

        static CamelliaLightEngine _CamelliaLightEngine;
        public static CamelliaLightEngine CamelliaLightEngine
        {
            get
            {
                if (_CamelliaLightEngine == null)
                    _CamelliaLightEngine = new CamelliaLightEngine();

                return _CamelliaLightEngine;
            }
        }

        static Cast5Engine _Cast5Engine;
        public static Cast5Engine Cast5Engine
        {
            get
            {
                if (_Cast5Engine == null)
                    _Cast5Engine = new Cast5Engine();

                return _Cast5Engine;
            }
        }

        static DesEngine _DesEngine;
        public static DesEngine DesEngine
        {
            get
            {
                if (_DesEngine == null)
                    _DesEngine = new DesEngine();

                return _DesEngine;
            }
        }

        static Gost28147Engine _Gost28147Engine;
        public static Gost28147Engine Gost28147Engine
        {
            get
            {
                if (_Gost28147Engine == null)
                    _Gost28147Engine = new Gost28147Engine();

                return _Gost28147Engine;
            }
        }

        static IdeaEngine _IdeaEngine;
        public static IdeaEngine IdeaEngine
        {
            get
            {
                if (_IdeaEngine == null)
                    _IdeaEngine = new IdeaEngine();

                return _IdeaEngine;
            }
        }

        static NoekeonEngine _NoekeonEngine;
        public static NoekeonEngine NoekeonEngine
        {
            get
            {
                if (_NoekeonEngine == null)
                    _NoekeonEngine = new NoekeonEngine();

                return _NoekeonEngine;
            }
        }

        static NullEngine _NullEngine;
        public static NullEngine NullEngine
        {
            get
            {
                if (_NullEngine == null)
                    _NullEngine = new NullEngine();

                return _NullEngine;
            }
        }

        static RC2Engine _RC2Engine;
        public static RC2Engine RC2Engine
        {
            get
            {
                if (_RC2Engine == null)
                    _RC2Engine = new RC2Engine();

                return _RC2Engine;
            }
        }

        static RC532Engine _RC532Engine;
        public static RC532Engine RC532Engine
        {
            get
            {
                if (_RC532Engine == null)
                    _RC532Engine = new RC532Engine();

                return _RC532Engine;
            }
        }

        static RC564Engine _RC564Engine;
        public static RC564Engine RC564Engine
        {
            get
            {
                if (_RC564Engine == null)
                    _RC564Engine = new RC564Engine();

                return _RC564Engine;
            }
        }

        static RC6Engine _RC6Engine;
        public static RC6Engine RC6Engine
        {
            get
            {
                if (_RC6Engine == null)
                    _RC6Engine = new RC6Engine();

                return _RC6Engine;
            }
        }

        static RijndaelEngine _RijndaelEngine;
        public static RijndaelEngine RijndaelEngine
        {
            get
            {
                if (_RijndaelEngine == null)
                    _RijndaelEngine = new RijndaelEngine(256);

                return _RijndaelEngine;
            }
        }

        static SeedEngine _SeedEngine;
        public static SeedEngine SeedEngine
        {
            get
            {
                if (_SeedEngine == null)
                    _SeedEngine = new SeedEngine();

                return _SeedEngine;
            }
        }

        static SerpentEngine _SerpentEngine;
        public static SerpentEngine SerpentEngine
        {
            get
            {
                if (_SerpentEngine == null)
                    _SerpentEngine = new SerpentEngine();

                return _SerpentEngine;
            }
        }

        static SkipjackEngine _SkipjackEngine;
        public static SkipjackEngine SkipjackEngine
        {
            get
            {
                if (_SkipjackEngine == null)
                    _SkipjackEngine = new SkipjackEngine();

                return _SkipjackEngine;
            }
        }

        static TeaEngine _TeaEngine;
        public static TeaEngine TeaEngine
        {
            get
            {
                if (_TeaEngine == null)
                    _TeaEngine = new TeaEngine();

                return _TeaEngine;
            }
        }

        static ThreefishEngine _ThreefishEngine256;
        public static ThreefishEngine ThreefishEngine256
        {
            get
            {
                if (_ThreefishEngine256 == null)
                    _ThreefishEngine256 = new ThreefishEngine(ThreefishEngine.BLOCKSIZE_256);

                return _ThreefishEngine256;
            }
        }

        static ThreefishEngine _ThreefishEngine512;
        public static ThreefishEngine ThreefishEngine512
        {
            get
            {
                if (_ThreefishEngine512 == null)
                    _ThreefishEngine512 = new ThreefishEngine(ThreefishEngine.BLOCKSIZE_512);

                return _ThreefishEngine512;
            }
        }

        static ThreefishEngine _ThreefishEngine1024;
        public static ThreefishEngine ThreefishEngine1024
        {
            get
            {
                if (_ThreefishEngine1024 == null)
                    _ThreefishEngine1024 = new ThreefishEngine(ThreefishEngine.BLOCKSIZE_1024);

                return _ThreefishEngine1024;
            }
        }

        static TnepresEngine _TnepresEngine;
        public static TnepresEngine TnepresEngine
        {
            get
            {
                if (_TnepresEngine == null)
                    _TnepresEngine = new TnepresEngine();

                return _TnepresEngine;
            }
        }

        static TwofishEngine _TwofishEngine;
        public static TwofishEngine TwofishEngine
        {
            get
            {
                if (_TwofishEngine == null)
                    _TwofishEngine = new TwofishEngine();

                return _TwofishEngine;
            }
        }

        static XteaEngine _XteaEngine;
        public static XteaEngine XteaEngine
        {
            get
            {
                if (_XteaEngine == null)
                    _XteaEngine = new XteaEngine();

                return _XteaEngine;
            }
        }

        public static byte[] PreFixSaltBytes(byte[] Salt, string Prefix)
        {
            if (string.IsNullOrEmpty(Prefix))
                return Salt;

            List<byte> by = new List<byte>();
            byte[] preSalt = Encoding.UTF8.GetBytes(Prefix);
            by.AddRange(preSalt);
            by.AddRange(Salt);
            return by.ToArray();
        }
    }
}
