using System.Text;
using Hotel.Domain.Interface;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using Hotel.Domain.Entities;
using Hotel.Application.Interfaces;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Services
{
    public class SerialService : ISerialService
    {
        //  public object vfncCriarChave;
        private readonly ISerialRepository _serialRepository;
        private readonly ITenantService _tenant;
        public Boolean booRegistrado;
        public Boolean booComercial;
        //   public Boolean booContaTempo;
      //  public int PrazoValidade = 370; // altere aqui o prazo de validade
      //  public int PrazoTrial = 370; // altere aqui o prazo de validade
        DateTime DataActual = DateTime.Now;
        private static int Contador = 0;

        public SerialService(ISerialRepository serialRepository, ITenantService tenant)
        {
            _serialRepository = serialRepository;
            _tenant = tenant;
            // _crypto = crysto;
        }

        public bool SetComercialFlag(bool valor)
        {
            booComercial = valor;
            return booComercial;
        }
        public int prazoValidade() => _serialRepository.Prazo();
        public int prazoTrial() => _serialRepository.Prazo();
        public long fncGeraRegistro(byte pos1, byte pos2, long k, int Serial)
        {
            // Dim varChave As Object
            object Id;
            object[] n = new object[4];
            byte j;
            // MsgBox(Crypto.Crypto2.Decrypt(objReg.RegRead(idx)))

            // SERIAL |  CONTADOR |   DATA INICIAL  | ULTIMO ACESSO  | PRAZO       |
            // 0    |     1     |        2        |       3        |   4         |
            // -------------------------------------------------------------------------

            //objReg = Interaction.CreateObject("wscript.shell");
            //  string regEntryIdx = My.Computer.Registry.GetValue(v_idx, "idx", null/* TODO Change to default(_) if this is not a reference type */);

            if (booComercial)
                Id = Serial;
            else
                // Gera chave de 8 dígitos baseado no número do HD
                Id = Serial;// fncChaveHD(8)

            // descarte 
            for (j = 1; j <= 8; j++)
            {
                if (!(j == pos1 | j == pos2))
                    n[0] = n[0] + Strings.Mid((string)Id, j, 1);
            }

            Id = n[0];
            // Soma
            n[0] = System.Convert.ToInt32(Strings.Left(Id.ToString(), 1)) + System.Convert.ToInt32(Strings.Mid(Id.ToString(), 2, 1));
            // Subtração
            n[1] = Math.Abs(System.Convert.ToInt32(Strings.Mid(Id.ToString(), 3, 1)) - System.Convert.ToInt32(Strings.Mid(Id.ToString(), 4, 1)));
            // Multiplicação
            n[2] = System.Convert.ToInt32(Strings.Mid(Id.ToString(), 5, 1)) * System.Convert.ToInt32(Strings.Mid(Id.ToString(), 6, 1));
            // Multiplicando pela constante
            //   string.Concat(n[0], n[0], n[0]);
            //    Id = long.Parse(n[0] & n[1] & n[2]) * k;
            Id = long.Parse(string.Concat(n[0], n[1], n[2])) * k;
            // Descartando o segundo dígito
            Id = Strings.Left(Id.ToString(), 1) + Strings.Mid(Id.ToString(), 3, Strings.Len(Id) - 2);
            // Soma
            n[0] = System.Convert.ToInt32(Strings.Left(Id.ToString(), 1)) + System.Convert.ToInt32(Strings.Mid(Id.ToString(), 2, 1));
            // Subtração
            n[1] = Math.Abs(System.Convert.ToInt32(Strings.Mid(Id.ToString(), 3, 1)) - System.Convert.ToInt32(Strings.Mid(Id.ToString(), 4, 1)));
            // Multiplicação
            n[2] = System.Convert.ToInt32(Strings.Mid(Id.ToString(), 5, 1)) * System.Convert.ToInt32(Strings.Mid(Id.ToString(), 6, 1));
            // Multiplicação pela constante, selecionando os 8 primeiros dígitos
            Id = (long.Parse(string.Concat(n[0], n[1], n[2])) * k, 1, 8).ToString().Substring(1, 8);
            // Resultado
            return (long)(Id);
        }

        public async Task<int> fncTempoDeBloqueio(int intPrazo)
        {
            var crypto = new CrystoService();
            //FncActulizaRegisto();
            var serial = await _serialRepository.GetByIdAsync(int.Parse(GetDriveSerialNumber()));
            int varTempo;
            string[] varReg;
            varReg = Decrypt(serial.ContadorData.ToString()).Split(",");       //Crypto2.Decrypt(vCampo3()).Split(",");
            varTempo = intPrazo - int.Parse(varReg[0]);
            return varTempo;

        }
        public  void fncReiniciar()
        {
          //  _serialRepository.Apagar();
            int cont = 0;
            var serial = new Serial(int.Parse(GetDriveSerialNumber()), string.Concat(cont.ToString(), ",", DataActual.Date.ToShortDateString()), DataActual.Date.ToShortDateString(), prazoValidade());
          //  await _serialRepository.AddAsync(serial);
            //addicionar o clone do serial no sistema
            object varNum;
            // serial.Id = 1;
            varNum = string.Concat(serial.NumSerial.ToString(), ",", cont.ToString(), "," + DataActual.Date.ToShortDateString(), ",", DataActual.Date.ToShortDateString()); // IIf(idcom = 0, fncCriarChave(4), idcom)                                                                                                                                                                   //  varNum = string.Concat((vCampo1()), ",", (vCampo3()), "," + vserial[1], ",", (vCampo4())); // IIf(idcom = 0, fncCriarChave(4), idcom)
            _serialRepository.UpdateSerialSistema(Encrypt(varNum.ToString())); //Crypto2.Encrypt

        }
        public async void fncCriarChaveRegWinAntiga()
        {
            var serial = await _serialRepository.GetByIdAsync(int.Parse(GetDriveSerialNumber()));
            object varNum;
            string[] vserial = Decrypt(serial.ContadorData.ToString()).Split(",");
            varNum = string.Concat(serial.NumSerial.ToString(), ",", serial.ContadorData.ToString(), "," + serial.DataInicial.ToString(), ",", serial.UltimoAcesso.ToString()); // IIf(idcom = 0, fncCriarChave(4), idcom)                                                                                                                                                                                //  varNum = string.Concat((vCampo1()), ",", (vCampo3()), "," + vserial[1], ",", (vCampo4())); // IIf(idcom = 0, fncCriarChave(4), idcom)

            // o resultado de varNum se assemelha com esta sequência: 2478,31,40436,7158
            // Grava parte da sequencia na tabela de registro.  isso servirá para sincronizar dados com o registro do windows
            _serialRepository.UpdateSerialSistema(Encrypt(varNum.ToString())); //Crypto2.Encrypt
            await _serialRepository.Update(serial);
        }
        public async void fncCriarChaveRegWin(int prazo = 0, long idcom = 0L)
        {
            object varNum;
            object varContador;
            varContador = 0;
            var serial = await _serialRepository.GetByIdAsync(int.Parse(GetDriveSerialNumber()));

            varNum = long.Parse(Strings.Mid(fncChave().ToString(), 5)); // Replace(fncCriarChave(4), 0, 2)
            varNum = string.Concat(varNum, ",", varContador, ",", DataActual.Date.ToShortDateString(), "," + DataActual.Date.ToShortDateString(), ",", GenerateRandomKey(4)).ToString(); // IIf(idcom = 0, fncCriarChave(4), idcom)

            serial.SetContadorData(Encrypt(string.Concat(varContador, ",", DataActual.Date.ToShortDateString())));
            serial.SetDataInicial(Encrypt(DataActual.Date.ToShortDateString()));
            serial.SetChave(Encrypt (GenerateDriveKey(4)));    
            serial.AtualizarUltimoAcesso();
            _serialRepository.UpdateSerialSistema(Encrypt(varNum.ToString())); //Crypto2.Encrypt
            await _serialRepository.Update(serial);
            //objReg = null;
        }

        public long fncRegistro(byte pos1, byte pos2, long k)
        {
            string decryptedSerial = _serialRepository.GetKeySerial();  //Crypto2.Decrypt(SerialSistema());
            if (string.IsNullOrEmpty(decryptedSerial))
            {
                return long.Parse(GenerateDriveKey(8)); // Default return for invalid serial
            }

            string[] serialComponents = decryptedSerial.Split(",");
            if (serialComponents.Length < 5)
            {
                return long.Parse(GenerateDriveKey(8)); // Default return for incomplete serial
            }

            object id;
            if (booComercial)
            {
                TimeSpan timeElapsed = DataActual - DateTime.Parse(serialComponents[2]);

                // Determine the ID based on expiration
                id = timeElapsed.Days > prazoValidade()
                    ? GenerateDriveKey(4) + serialComponents[4]
                    : GenerateDriveKey(4) + serialComponents[0];
            }
            else
            {
                // Generate an 8-digit key based on the HD number
                id = GenerateDriveKey(8);
            }

            return (long)id;
        }

        public object fncChave()
        {
            object fncChaveRet = default;

            // Decrypt the serial and split it into components
            string decryptedSerial = Decrypt(_serialRepository.GetKeySerial());//       Crypto2.Decrypt(SerialSistema());
            if (string.IsNullOrEmpty(Decrypt(_serialRepository.GetKeySerial())))
            {
                return GenerateDriveKey(8); // Default return for invalid serial
            }

            string[] serialComponents = decryptedSerial.Split(",");
            if (serialComponents.Length < 5)
            {
                return GenerateDriveKey(8); // Default return for incomplete serial
            }

            // Calculate the time difference from the serial's date
            TimeSpan timeElapsed = DataActual - DateTime.Parse(serialComponents[2]);

            if (booComercial)
            {
                // Check if the serial has expired
                if (timeElapsed.Days > prazoValidade())
                {
                    fncChaveRet = GenerateDriveKey(4) + serialComponents[4];
                }
                else
                {
                    fncChaveRet = GenerateDriveKey(4) + serialComponents[0];
                }
            }
            else
            {
                fncChaveRet = GenerateDriveKey(8);
            }

            return fncChaveRet;
        }
        public bool IsTimeExpiredAsync(int maxDays)
        {
            try
            {
                // Decifra e divide o serial em partes
                string[] serialParts = Decrypt(_serialRepository.GetKeySerial()).Split(",");

                // Calcula o tempo decorrido desde a data inicial do serial
                TimeSpan elapsed = DataActual - DateTime.Parse(serialParts[2]);
                Contador = elapsed.Days;

                // Verifica se a data inicial é maior que a data atual
                if (DateTime.Parse(serialParts[2]) > DataActual)
                {
                    throw new InvalidOperationException(
                        "Detectamos uma alteração na data do computador para uma data anterior. " +
                        "O sistema só voltará a funcionar quando a data correta for restaurada.");
                }

                // Verifica se a data do último acesso é maior que a data atual
                if (DateTime.Parse(serialParts[3]) > DataActual)
                {
                    throw new InvalidOperationException(
                        "Detectamos uma alteração na data do computador para uma data anterior. " +
                        "O sistema só voltará a funcionar quando a data correta for restaurada.");
                }

                // Verifica se o contador excede o prazo de validade
                if (Contador > maxDays)
                {
                    throw new ArgumentException(
                        "A sua licença expirou. Contate a empresa para adquirir uma nova licença.");
                }

                // Atualiza o registro se tudo estiver correto
                //   await FncActulizaRegisto();
                return false;
            }
            catch (InvalidOperationException ex)
            {
                // Opcional: registre logs para diagnósticos
                Console.WriteLine($"Erro crítico: {ex.Message}");
                throw; // Rethrow the exception to propagate it further
            }
            catch (ArgumentException ex)
            {
                // Opcional: registre logs para diagnósticos
                Console.WriteLine($"Aviso: {ex.Message}");
                throw; // Rethrow the exception to propagate it further
            }
            catch (Exception ex)
            {
                // Opcional: registre logs para diagnósticos
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                throw new ApplicationException("Ocorreu um erro inesperado ao verificar a licença.", ex);
            }
        }


/*         private void ShowCriticalMessage(string message)
        {
            Interaction.MsgBox(message, MsgBoxStyle.Critical, "Erro Crítico");
        } */
 

        public async Task<bool> ValidateLicenseComAsync(int maxDays) //fncValidadeCom
        {
            bool isValid = false;
            booComercial = true;
            string decryptedSerial = Decrypt(_serialRepository.GetKeySerial());

            if (string.IsNullOrEmpty(decryptedSerial))
            {
                // Caso a licença não esteja presente ou inválida
                HandleInvalidLicense();
                return false;
            }

            var serialData = await _serialRepository.GetByIdAsync(int.Parse(GetDriveSerialNumber()));
            string[] licenseData = Decrypt(serialData.ContadorData).Split(",");
            string[] serialParts = decryptedSerial.Split(",");

            // Verifica se a contagem de dias é válida
            if (int.Parse(licenseData[0]) > 0)
            {
                HandleInvalidLicense();
                return false;
            }

            // Calcula o tempo decorrido
            DateTime licenseDate = DateTime.Parse(serialParts[2]);
            int elapsedDays = (DataActual - licenseDate).Days;

            // Verifica validade da licença
            if (elapsedDays <= maxDays && CompareLicenseData(licenseData, serialParts))
            {
                isValid = HasLicenseExpired(maxDays);
            }

            return isValid;
        }

        public async Task<bool> ValidateLicenseAsync()
        {
            booComercial= false;
            // Inicializa o valor de retorno
            bool isLicenseValid;
            // Obtém o serial decifrado
            string decryptedSerial = Decrypt(_serialRepository.GetKeySerial());

            // Obtém os dados do serial a partir do repositório
            var serialData = await _serialRepository.GetByIdAsync(int.Parse(GetDriveSerialNumber()));
            string[] licenseData = Decrypt(serialData.ContadorData).Split(",");

            // Verifica se o serial está vazio
            if (string.IsNullOrEmpty(decryptedSerial))
            {
                string[] accessData = Decrypt(serialData.ContadorData).Split(",");

                if (int.Parse(accessData[0]) > 0)
                {
                    // Acesso não autorizado (novo usuário ou dados inconsistentes)
                    isLicenseValid = booRegistrado; // Registro inválido
                }
                else
                {
                    // Cria nova chave no registro (primeiro uso do aplicativo)
                    fncCriarChaveRegWin(0);
                    isLicenseValid = true;
                }
            }
            else
            {
                // Atualiza os registros existentes
                await FncActulizaRegisto();

                // Calcula o tempo decorrido
                string[] serialParts = decryptedSerial.Split(",");
                TimeSpan elapsed = DataActual - DateTime.Parse(serialParts[2]);
                Contador = elapsed.Days;

                // Verifica se as datas no serial coincidem com os dados de acesso
                /* if (DateTime.Parse(serialParts[2]) != DateTime.Parse(licenseData[1]))
                {
                    isLicenseValid = booRegistrado; // Dados inconsistentes
                } */
                if (int.Parse(serialParts[1]) >= _serialRepository.Prazo())
                {
                    // Licença expirada
                    isLicenseValid = booRegistrado;
                    throw new ApplicationException("A sua licença expirou. Contate a empresa para adquirir uma nova licença.");
                    
                }
                else
                {
                    // Verifica consistência dos dados
                    string concatenatedData = $"{serialParts[1]},{serialParts[3]}";
                    if (concatenatedData == string.Join(",", licenseData))
                    {
                        // Verifica se o tempo está esgotado
                        if (IsTimeExpiredAsync(_serialRepository.Prazo()))
                        {
                            isLicenseValid = booRegistrado;
                        }
                        else
                        {
                            isLicenseValid = true;
                        }
                    }
                    else
                    {
                        // Dados inconsistentes
                        isLicenseValid = booRegistrado;
                        throw new ApplicationException("Problema com sua licença. Contate a empresa para resolver o problema.");
                        
                    }
                }
            }

            return isLicenseValid;
        }

       
        public Boolean fncRegistrado()
        {
            bool vReg = false;
           /*  fncRegistro(1, 3, 76476).ToString();
            if (fncRegistro(1, 3, 76476).ToString() == Decrypt(GetDriveSerialNumber()))
            {
                vReg = true;
            } */
            return vReg;
        }
        private void HandleInvalidLicense()
        {
            fncCriarChaveRegWinAntiga();
            // Adicionar lógica adicional para lidar com licenças inválidas, se necessário
        }

        private bool CompareLicenseData(string[] licenseData, string[] serialParts)
        {
            return string.Concat(serialParts[1], ",", serialParts[2]) == string.Join(",", licenseData);
        }

        private bool HasLicenseExpired(int maxDays)
        {
            return Contador > maxDays || IsTimeExpiredAsync(maxDays);
        }

        private async Task<int> FncActulizaRegisto()
        {
            int FncActulizaRegistoRet = 0;

            string[] vserial = Decrypt(_serialRepository.GetKeySerial()).Split(",");
            var serialData = await _serialRepository.GetByIdAsync(int.Parse(GetDriveSerialNumber()));
            TimeSpan ts = DataActual - DateTime.Parse(vserial[2]);
            string updatedSerialValue = vserial[0] + "," + ts.Days + "," + vserial[2] + "," + DataActual.Date.ToShortDateString() + "," + GenerateDriveKey(4);


            _serialRepository.UpdateSerialSistema(Encrypt(updatedSerialValue));  //Crypto2.Encrypt
            serialData.SetContadorData(Encrypt(ts.Days + "," + DataActual.Date.ToShortDateString()));
            serialData.SetChave(Encrypt (GenerateDriveKey(4)));    //(vserial[4]));
            serialData.AtualizarUltimoAcesso();
            await _serialRepository.Update(serialData);
            return FncActulizaRegistoRet;
        }


        public long GenerateRegister(byte pos1, byte pos2, long k, int serial)
        {
            // Initialize variables
            string id = booComercial ? serial.ToString() : serial.ToString(); // Replace with `GenerateHDKey(8)` if applicable
            StringBuilder n = new StringBuilder();

            // Discard unwanted positions
            for (byte j = 1; j <= 8; j++)
            {
                if (j != pos1 && j != pos2)
                {
                    n.Append(id.Substring(j - 1, 1));
                }
            }

            // Perform initial transformations
            id = n.ToString();
            int n0 = int.Parse(id.Substring(0, 1)) + int.Parse(id.Substring(1, 1));
            int n1 = Math.Abs(int.Parse(id.Substring(2, 1)) - int.Parse(id.Substring(3, 1)));
            int n2 = int.Parse(id.Substring(4, 1)) * int.Parse(id.Substring(5, 1));

            // Concatenate results and multiply by the constant
            id = (long.Parse($"{n0}{n1}{n2}") * k).ToString();

            // Discard the second digit
            id = id[0] + id.Substring(2);

            // Recalculate transformations
            n0 = int.Parse(id.Substring(0, 1)) + int.Parse(id.Substring(1, 1));
            n1 = Math.Abs(int.Parse(id.Substring(2, 1)) - int.Parse(id.Substring(3, 1)));
            n2 = int.Parse(id.Substring(4, 1)) * int.Parse(id.Substring(5, 1));

            // Concatenate results, multiply by the constant, and extract the first 8 digits
            id = (long.Parse($"{n0}{n1}{n2}") * k).ToString();
            id = id.Substring(0, 8);

            // Return the final result
            return long.Parse(id);
        }

        public string GenerateDriveKey(byte digitCount) //fncChaveHD
        {
            // Get the first 8 characters of the drive serial number
            string driveId = GetDriveSerialNumber().ToString().Substring(0, 8);

            // Return the requested number of digits (8 or 4), defaulting to 8 if invalid
            return digitCount switch
            {
                8 => driveId,
                4 => driveId.Substring(0, 4),
                _ => driveId,
            };
        }

        public string GetDriveSerialNumber()
        {
            return "20241204";
        }

        public string GenerateRandomKey(int digitCount) // fncCriarChave
        {
            // Ensure the digit count is capped at 12
            digitCount = Math.Min(digitCount, 12);

            string randomKey = null;

            try
            {
                for (int i = 0; i < 12; i++) // Attempt 12 times to create a valid key
                {
                    // Generate a random number with the specified number of digits
                    VBMath.Randomize();
                    int maxValue = (int)Math.Pow(10, digitCount);
                    int randomValue = (int)(VBMath.Rnd() * maxValue);

                    // Check if the length matches the required digit count
                    if (randomValue.ToString().Length == digitCount)
                    {
                        randomKey = randomValue.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }

            return randomKey;
        }

        private const string Key = "MNBVCXZLKJ039RTY4U6W"; // Key must be securely stored
        private const string IV = "1234567890123456";      // Initialization Vector (must be 16 bytes for AES)

        // Generate a key using SHA256 (for a fixed 256-bit key)
        private static byte[] GenerateKey(string key)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }

        // Encrypt a string using AES
        public static string Encrypt(string stringToEncrypt)
        {
            try
            {
                return stringToEncrypt;
                 /*   using (var aes = Aes.Create())
                  {
                      aes.Key = GenerateKey(Key);
                      aes.IV = Encoding.UTF8.GetBytes(IV);
                      aes.Mode = CipherMode.CBC; // CBC is more secure than ECB
                      aes.Padding = PaddingMode.PKCS7;

                      var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                       var buffer = Encoding.UTF8.GetBytes(stringToEncrypt);

                        byte[] encrypted = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                      return stringToEncrypt; //Convert.ToBase64String(encrypted);
                  }  */
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Encryption failed.", ex);
            }
        }
        // Decrypt a string using AES
        public static string Decrypt(string encryptedString)
        {
            try
            {
                return encryptedString;
                
              /*   using (var aes = Aes.Create())
                {
                     aes.Key = GenerateKey(Key);
                    aes.IV = Encoding.UTF8.GetBytes(IV);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV); 
                       var buffer = Convert.FromBase64String(encryptedString);

                      byte[] decrypted = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                    return encryptedString; // Encoding.UTF8.GetString(decrypted);
                } */
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Decryption failed.", ex);
            }
        }

    }
}