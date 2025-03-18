using CaseStage.DetalheProcesso;

namespace CaseStage.Processos;

public record ProcessoComDetalhesDto(
    Guid Id,
    string Nome,
    string Descricao,
    Guid AreaId,
    string AreaNome,
    Guid? ProcessoPaiId,
    TipoProcesso Tipo,
    StatusProcesso Status,
    Guid? DetalheId,
    List<FerramentaProcessoDto> Ferramentas,
    List<ResponsavelProcessoDto> Responsaveis,
    List<DocumentoProcessoDto> Documentos
);