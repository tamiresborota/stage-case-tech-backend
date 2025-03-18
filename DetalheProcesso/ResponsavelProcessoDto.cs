namespace CaseStage.DetalheProcesso;

public record ResponsavelProcessoDto(
    Guid Id,
    string Nome
);

public record AddResponsavelRequest(
    string Nome
);

public record UpdateResponsavelRequest(
    string Nome
);