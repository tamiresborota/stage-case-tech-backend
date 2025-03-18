// ProcessoDetalhesRoutes.cs

using CaseStage.Data;
using CaseStage.DetalheProcesso;
using Microsoft.EntityFrameworkCore;

public static class ProcessoDetalhesRoutes
{
    public static void AddProcessoDetalhesRoutes(this WebApplication app)
    {
        var rotasProcessoDetalhes = app.MapGroup("processos/{processoId}/detalhes");

        rotasProcessoDetalhes.MapGet("", async (Guid processoId, AppDbContext context, CancellationToken ct) =>
        {
            var processoExiste = await context.Processos
                .AnyAsync(p => p.Id == processoId && !p.IsDeleted, ct);

            if (!processoExiste)
                return Results.NotFound("Processo não encontrado");

            var detalhe = await context.ProcessosDetalhes
                .Include(processoDetalhe => processoDetalhe.Responsaveis)
                .Include(processoDetalhe => processoDetalhe.Ferramentas)
                .Include(processoDetalhe => processoDetalhe.Documentos)
                .FirstOrDefaultAsync(pd => pd.ProcessoId == processoId && !pd.IsDeleted, ct);

            if (detalhe == null)
            {
                // Criar um novo detalhamento se não existir
                detalhe = new ProcessoDetalhe(processoId);
                context.ProcessosDetalhes.Add(detalhe);
                await context.SaveChangesAsync(ct);
            }

            var response = new ProcessoDetalheDto(
                detalhe.Id,
                detalhe.ProcessoId,
                detalhe.Ferramentas.Select(f => new FerramentaProcessoDto(
                    f.Id,
                    f.Nome
                )).ToList(),
                detalhe.Responsaveis.Select(r => new ResponsavelProcessoDto(
                    r.Id,
                    r.Nome
                )).ToList(),
                detalhe.Documentos.Select(d => new DocumentoProcessoDto(
                    d.Id,
                    d.Nome
                )).ToList()
            );

            return Results.Ok(response);
        });

        var rotasFerramentas = rotasProcessoDetalhes.MapGroup("ferramentas");

        // Adicionar ferramenta
        rotasFerramentas.MapPost("",
            async (Guid processoId, AddFerramentaRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var detalhe = await context.ProcessosDetalhes
                    .Include(processoDetalhe => processoDetalhe.Responsaveis)
                    .Include(processoDetalhe => processoDetalhe.Ferramentas)
                    .Include(processoDetalhe => processoDetalhe.Documentos)
                    .FirstOrDefaultAsync(pd => pd.ProcessoId == processoId && !pd.IsDeleted, ct);

                if (detalhe == null)
                {
                    var processoExiste = await context.Processos
                        .AnyAsync(p => p.Id == processoId && !p.IsDeleted, ct);

                    if (!processoExiste)
                        return Results.NotFound("Processo não encontrado");

                    detalhe = new ProcessoDetalhe(processoId);
                    context.ProcessosDetalhes.Add(detalhe);
                    await context.SaveChangesAsync(ct);
                }

                var ferramenta = new FerramentaProcesso(
                    request.Nome,
                    detalhe.Id
                );

                context.FerramentasProcesso.Add(ferramenta);
                await context.SaveChangesAsync(ct);

                return Results.Ok(new FerramentaProcessoDto(
                    ferramenta.Id,
                    ferramenta.Nome
                ));
            });

        // Atualizar ferramenta
        rotasFerramentas.MapPut("{ferramentaId}",
            async (Guid processoId, Guid ferramentaId, UpdateFerramentaRequest request, AppDbContext context,
                CancellationToken ct) =>
            {
                var ferramenta = await context.FerramentasProcesso
                    .FirstOrDefaultAsync(f => f.Id == ferramentaId &&
                                              f.ProcessoDetalhe.ProcessoId == processoId &&
                                              !f.ProcessoDetalhe.IsDeleted, ct);

                if (ferramenta == null)
                    return Results.NotFound("Ferramenta não encontrada");

                ferramenta.AlterarNome(request.Nome);

                await context.SaveChangesAsync(ct);

                return Results.Ok(new FerramentaProcessoDto(
                    ferramenta.Id,
                    ferramenta.Nome
                ));
            });

        // Excluir ferramenta
        rotasFerramentas.MapDelete("{ferramentaId}",
            handler: async (Guid processoId, Guid ferramentaId, AppDbContext context, CancellationToken ct) =>
            {
                var ferramenta = await context.FerramentasProcesso
                    .FirstOrDefaultAsync(f => f.Id == ferramentaId &&
                                              f.ProcessoDetalhe.ProcessoId == processoId &&
                                              !f.ProcessoDetalhe.IsDeleted, ct);

                if (ferramenta == null)
                    return Results.NotFound("Ferramenta não encontrada");

                context.FerramentasProcesso.Remove(ferramenta);
                await context.SaveChangesAsync(ct);

                return Results.Ok(new { message = "Ferramenta removida com sucesso" });
            });

        // Responsáveis
        var rotasResponsaveis = rotasProcessoDetalhes.MapGroup("responsaveis");

        // Adicionar responsável
        rotasResponsaveis.MapPost("",
            async (Guid processoId, AddResponsavelRequest request, AppDbContext context,
                CancellationToken ct) =>
            {
                var detalhe = await context.ProcessosDetalhes
                    .FirstOrDefaultAsync(pd => pd.ProcessoId == processoId && !pd.IsDeleted, ct);

                if (detalhe == null)
                {
                    var processoExiste = await context.Processos
                        .AnyAsync(p => p.Id == processoId && !p.IsDeleted, ct);

                    if (!processoExiste)
                        return Results.NotFound("Processo não encontrado");

                    detalhe = new ProcessoDetalhe(processoId);
                    context.ProcessosDetalhes.Add(detalhe);
                    await context.SaveChangesAsync(ct);
                }

                var responsavel = new ResponsavelProcesso(
                    request.Nome,
                    detalhe.Id
                );

                context.ResponsaveisProcesso.Add(responsavel);
                await context.SaveChangesAsync(ct);

                return Results.Ok(new ResponsavelProcessoDto(
                    responsavel.Id,
                    responsavel.Nome
                ));
            });

        // Atualizar responsável
        rotasResponsaveis.MapPut("{responsavelId}",
            async (Guid processoId, Guid responsavelId, UpdateResponsavelRequest request, AppDbContext context,
                CancellationToken ct) =>
            {
                var responsavel = await context.ResponsaveisProcesso
                    .FirstOrDefaultAsync(r => r.Id == responsavelId &&
                                              r.ProcessoDetalhe.ProcessoId == processoId &&
                                              !r.ProcessoDetalhe.IsDeleted, ct);

                if (responsavel == null)
                    return Results.NotFound("Responsável não encontrado");

                responsavel.AlterarNome(request.Nome);

                await context.SaveChangesAsync(ct);

                return Results.Ok(new ResponsavelProcessoDto(
                    responsavel.Id,
                    responsavel.Nome
                ));
            });

        // Excluir responsável
        rotasResponsaveis.MapDelete("{responsavelId}",
            async (Guid processoId, Guid responsavelId, AppDbContext context, CancellationToken ct) =>
            {
                var responsavel = await context.ResponsaveisProcesso
                    .FirstOrDefaultAsync(r => r.Id == responsavelId &&
                                              r.ProcessoDetalhe.ProcessoId == processoId &&
                                              !r.ProcessoDetalhe.IsDeleted, ct);

                if (responsavel == null)
                    return Results.NotFound("Responsável não encontrado");

                context.ResponsaveisProcesso.Remove(responsavel);
                await context.SaveChangesAsync(ct);

                return Results.Ok(new { message = "Responsável removido com sucesso" });
            });

        // Documentos
        var rotasDocumentos = rotasProcessoDetalhes.MapGroup("documentos");

        // Adicionar documento
        rotasDocumentos.MapPost("",
            async (Guid processoId, AddDocumentoRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var detalhe = await context.ProcessosDetalhes
                    .FirstOrDefaultAsync(pd => pd.ProcessoId == processoId && !pd.IsDeleted, ct);

                if (detalhe == null)
                {
                    var processoExiste = await context.Processos
                        .AnyAsync(p => p.Id == processoId && !p.IsDeleted, ct);

                    if (!processoExiste)
                        return Results.NotFound("Processo não encontrado");

                    detalhe = new ProcessoDetalhe(processoId);
                    context.ProcessosDetalhes.Add(detalhe);
                    await context.SaveChangesAsync(ct);
                }

                var documento = new DocumentoProcesso(
                    request.Nome,
                    detalhe.Id
                );

                context.DocumentosProcesso.Add(documento);
                await context.SaveChangesAsync(ct);

                return Results.Ok(new DocumentoProcessoDto(
                    documento.Id,
                    documento.Nome
                ));
            });

        // Atualizar documento
        rotasDocumentos.MapPut("{documentoId}",
            async (Guid processoId, Guid documentoId, UpdateDocumentoRequest request, AppDbContext context,
                CancellationToken ct) =>
            {
                var documento = await context.DocumentosProcesso
                    .FirstOrDefaultAsync(d => d.Id == documentoId &&
                                              d.ProcessoDetalhe.ProcessoId == processoId &&
                                              !d.ProcessoDetalhe.IsDeleted, ct);

                if (documento == null)
                    return Results.NotFound("Documento não encontrado");

                documento.AlterarNome(request.Nome);

                await context.SaveChangesAsync(ct);

                return Results.Ok(new DocumentoProcessoDto(
                    documento.Id,
                    documento.Nome
                ));
            });

        // Excluir documento
        rotasDocumentos.MapDelete("{documentoId}",
            async (Guid processoId, Guid documentoId, AppDbContext context, CancellationToken ct) =>
            {
                var documento = await context.DocumentosProcesso
                    .FirstOrDefaultAsync(d => d.Id == documentoId &&
                                              d.ProcessoDetalhe.ProcessoId == processoId &&
                                              !d.ProcessoDetalhe.IsDeleted, ct);

                if (documento == null)
                    return Results.NotFound("Documento não encontrado");

                context.DocumentosProcesso.Remove(documento);
                await context.SaveChangesAsync(ct);

                return Results.Ok(new { message = "Documento removido com sucesso" });
            });
    }
}