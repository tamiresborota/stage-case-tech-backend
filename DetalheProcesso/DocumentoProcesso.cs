namespace CaseStage.DetalheProcesso;

public class DocumentoProcesso
{
    public Guid Id { get; set; }
    public Guid ProcessoDetalheId { get; set; }
    public ProcessoDetalhe ProcessoDetalhe { get; set; }
    public string Nome { get; set; }

    public DocumentoProcesso(string nome, Guid processoDetalheId)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        ProcessoDetalheId = processoDetalheId;
    }
    
    public void AlterarNome(string nome)
    {
        Nome = nome;
    }
    
}