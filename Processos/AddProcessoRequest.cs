namespace CaseStage.Processos;

public record AddProcessoRequest(
    string Nome, 
    string Descricao, 
    Guid AreaId, 
    Guid? ProcessoPaiId = null, 
    TipoProcesso Tipo = TipoProcesso.Manual, 
    StatusProcesso Status = StatusProcesso.Implementado
);
