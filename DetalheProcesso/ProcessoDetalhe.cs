using CaseStage.DetalheProcesso;
using CaseStage.Processos;

public class ProcessoDetalhe
{
    public Guid Id { get; set; }
    public Guid ProcessoId { get; set; }
    public Processo Processo { get; set; }
    public bool IsDeleted { get; set; }
    
    public List<FerramentaProcesso> Ferramentas { get; set; } = new List<FerramentaProcesso>();
    public List<ResponsavelProcesso> Responsaveis { get; set; } = new List<ResponsavelProcesso>();
    public List<DocumentoProcesso> Documentos { get; set; } = new List<DocumentoProcesso>();
    
    public ProcessoDetalhe(Guid processoId)
    {
        Id = Guid.NewGuid();
        ProcessoId = processoId;
        IsDeleted = false;
    }
    
    public void Desativar()
    {
        IsDeleted = true;
    }
}
