using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Services.Interfaces;

namespace Services.Data
{
    public class EncryptedStringConverter : ValueConverter<string, string>
    {
        public EncryptedStringConverter(IEncryptionService encryptionService, ConverterMappingHints? mappingHints = null)
        : base(
            v => encryptionService.Encrypt(v),
            v => encryptionService.Decrypt(v),
            mappingHints)
        {
        }
    }
}
