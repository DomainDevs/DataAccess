using System.ComponentModel.DataAnnotations;

namespace SolucionDA.Models;

public class Cliente
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
}