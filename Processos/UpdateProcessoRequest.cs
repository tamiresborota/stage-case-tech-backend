namespace CaseStage.Processos;

public record UpdateProcessoRequest(
    string Nome,
    string Descricao,
    Guid? ProcessoPaiId,
    TipoProcesso Tipo,
    StatusProcesso Status
);