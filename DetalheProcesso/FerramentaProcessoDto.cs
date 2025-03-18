namespace CaseStage.DetalheProcesso;

public record FerramentaProcessoDto(
    Guid Id,
    string Nome
);

public record AddFerramentaRequest(
    string Nome
);

public record UpdateFerramentaRequest(
    string Nome
);
