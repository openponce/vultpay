using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using VultPay.Infra.Core.Security;
using System;
using System.Text;

namespace Org.Cerberus.Security.Cryptography
{
    public class BCEngineCore
    {
        Encoding _encoding;
        IBlockCipher _blockCipher;
        private PaddedBufferedBlockCipher _cipher;
        private IBlockCipherPadding _padding;
        KeyParameter KeyParameter;

        #region Public Methods

        public string TwoFishEncryption(string TextPlain, string Password, byte[] Salt, string Prefix = "")
        {
            TwoFish(TextPlain, Password, Salt, Prefix);
            return Encrypt(TextPlain, KeyParameter);
        }

        public string TwoFishDecryption(string TextEncripted, string Password, byte[] Salt, string Prefix = "")
        {
            TwoFish(TextEncripted, Password, Salt, Prefix);
            return Decrypt(TextEncripted, KeyParameter);
        }

        #endregion

        #region Private Methods

        void TwoFish(string TextPlain, string Password, byte[] Salt, string Prefix = "")
        {

            Sha3Digest Sha3Digest = new Sha3Digest((int)KeySizes.F256);
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(Sha3Digest);
            gen.Init(Encoding.UTF8.GetBytes(Password), BCEngines.PreFixSaltBytes(Salt, Prefix), Constants.NUM_ITERATOR);
            KeyParameter = (KeyParameter)gen.GenerateDerivedParameters(BCEngines.TwofishEngine.AlgorithmName, (int)KeySizes.F256);

            SetPadding(new Pkcs7Padding());
            SetBlockCipher(BCEngines.TwofishEngine);
            SetEncoding(Encoding.UTF8);
        }

        void SetPadding(IBlockCipherPadding padding)
        {
            if (padding != null)
                _padding = padding;
            else
                throw new NullReferenceException("Padding is null!");
        }

        void SetBlockCipher(IBlockCipher blockCipher)
        {
            if (blockCipher != null)
                _blockCipher = blockCipher;
            else
                throw new NullReferenceException("BlockCipher is null!");
        }

        void SetEncoding(Encoding encoding)
        {
            if (encoding != null)
                _encoding = encoding;
            else
                throw new NullReferenceException("Encoding is null!");
        }

        string Encrypt(string plain, ICipherParameters SetKeyParameter)
        {
            byte[] result = BouncyCastleCrypto(true, _encoding.GetBytes(plain), SetKeyParameter);
            return Convert.ToBase64String(result);
        }

        string Decrypt(string cipher, ICipherParameters SetKeyParameter)
        {
            byte[] result = BouncyCastleCrypto(false, Convert.FromBase64String(cipher), SetKeyParameter);
            return _encoding.GetString(result, 0, result.Length);
        }

        byte[] BouncyCastleCrypto(bool forEncrypt, byte[] input, ICipherParameters SetKeyParameter)
        {
            try
            {
                _cipher = _padding == null
                    ? new PaddedBufferedBlockCipher(_blockCipher)
                    : new PaddedBufferedBlockCipher(_blockCipher, _padding);

                _cipher.Init(forEncrypt, SetKeyParameter);

                byte[] ret = _cipher.DoFinal(input);
                return ret;

            }
            catch
            {
                // throw new CryptoException(ex);
            }
            return null;
        }

        #endregion
    }

    public sealed class BCEngine
    {
        private static BCEngineCore instance;

        private BCEngine() { }

        public static BCEngineCore Instance
        {
            get
            {
                if (instance == null)
                    instance = new BCEngineCore();

                return instance;
            }
        }
    }
}