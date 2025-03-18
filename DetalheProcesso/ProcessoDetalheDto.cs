using CaseStage.DetalheProcesso;

public record ProcessoDetalheDto(
    Guid Id,
    Guid ProcessoId,
    List<FerramentaProcessoDto> Ferramentas,
    List<ResponsavelProcessoDto> Responsaveis,
    List<DocumentoProcessoDto> Documentos
);


