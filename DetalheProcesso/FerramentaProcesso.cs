namespace CaseStage.DetalheProcesso;

public class FerramentaProcesso
{
    public Guid Id { get; set; }
    public Guid ProcessoDetalheId { get; set; }
    public ProcessoDetalhe ProcessoDetalhe { get; set; }
    public string Nome { get; set; }

    public FerramentaProcesso(string nome, Guid processoDetalheId)
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