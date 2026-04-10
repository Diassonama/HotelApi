
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
namespace Hotel.Domain.Enums
{
/*     public enum Situacao
    {
        Livre = 'L',
        Ocupado = 'O',
        Manuntencao = 'M',
        Atrasado = 'A',
        Hoje = 'H',
        Amanha = 'H',
        Limpeza = 'Z',
        Bloqueado = 'B'
    } */


   [JsonConverter(typeof(JsonStringEnumConverter))]
public enum Situacao
{
    [EnumMember(Value = "Livre")]
    Livre,

    [EnumMember(Value = "Ocupado")]
    Ocupado,

    [EnumMember(Value = "Manuntencao")]
    Manuntencao,

    [EnumMember(Value = "Atrasado")]
    Atrasado,

    [EnumMember(Value = "Hoje")]
    Hoje,

    [EnumMember(Value = "Amanha")]
    Amanha,

    [EnumMember(Value = "Limpeza")]
    Limpeza,

    [EnumMember(Value = "Reservado")]
    Reservado,

    [EnumMember(Value = "Bloqueado")]
    Bloqueado
} 

}