using System.ComponentModel.DataAnnotations;

namespace Lab.Api.Application.DTOs.Appointments;

public class UpsertAppointmentDto
{
    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }

    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Informe o ID do cliente.")]
    public Guid CustomerId { get; set; }
    public Guid? ServiceId { get; set; }

    [Required(ErrorMessage = "Informe a data e hora de início.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Informe a data e hora de término.")]
    public DateTime EndDate { get; set; }
}
