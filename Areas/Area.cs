namespace CaseStage.Areas;

public class Area
{
    public Guid Id { get; init; }
    public string Nome { get; private set; }
    public bool IsDeleted { get; set; } = false;

    public Area(string nome)
    {
        Id = Guid.NewGuid();
        Nome = nome;
    }

    public void AlterarName(string nome)
    {
        Nome = nome;
    }

    public void Desativar(){
        IsDeleted = true;
    }
}