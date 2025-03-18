namespace CaseStage.Processos;

public class ProcessoHierarquiaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public TipoProcesso Tipo { get; set; }
    public StatusProcesso Status { get; set; }
    public string Area { get; set; }
    public List<ProcessoHierarquiaDto> SubProcessos { get; set; } = new();
}