namespace CaseStage.DetalheProcesso;

public record DocumentoProcessoDto(
    Guid Id,
    string Nome
);

public record AddDocumentoRequest(
    string Nome
);

public record UpdateDocumentoRequest(
    string Nome
);