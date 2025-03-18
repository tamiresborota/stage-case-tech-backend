using CaseStage.Data;
using CaseStage.DetalheProcesso;
using Microsoft.EntityFrameworkCore;

namespace CaseStage.Processos;

public static class ProcessosRoutes
{
    public static void AddProcessosRoutes(this WebApplication app)
    {
        var rotasProcessos = app.MapGroup("processos");

        rotasProcessos.MapPost("", async (AddProcessoRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var areaExiste = await context.Areas.AnyAsync(a => a.Id == request.AreaId, ct);
            if (!areaExiste)
                return Results.NotFound("Área não encontrada");

            if (request.ProcessoPaiId.HasValue)
            {
                var processoPaiExiste = await context.Processos
                    .AnyAsync(p => p.Id == request.ProcessoPaiId && !p.IsDeleted, ct);

                if (!processoPaiExiste)
                    return Results.NotFound("Processo pai não encontrado");
            }

            var jaExiste = await context.Processos
                .Where(p => !p.IsDeleted)
                .AnyAsync(p => p.Nome == request.Nome && p.AreaId == request.AreaId, ct);

            if (jaExiste)
                return Results.Conflict("Já existe um processo com este nome na mesma área");

            Processo novoProcesso;
            if (request.ProcessoPaiId.HasValue)
            {
                novoProcesso = new Processo(
                    request.Nome,
                    request.Descricao,
                    request.AreaId,
                    request.ProcessoPaiId.Value,
                    request.Tipo,
                    request.Status
                );
            }
            else
            {
                novoProcesso = new Processo(
                    request.Nome,
                    request.Descricao,
                    request.AreaId,
                    request.Tipo,
                    request.Status
                );
            }

            await context.Processos.AddAsync(novoProcesso, ct);
            await context.SaveChangesAsync(ct);

            var processoRetorno = new ProcessoDto(
                novoProcesso.Id,
                novoProcesso.Nome,
                novoProcesso.Descricao,
                novoProcesso.AreaId,
                novoProcesso.ProcessoPaiId,
                novoProcesso.Tipo,
                novoProcesso.Status
            );

            return Results.Ok(processoRetorno);
        });


        rotasProcessos.MapGet("", async (AppDbContext context, CancellationToken ct) =>
        {
            var processos = await context.Processos
                .Where(p => !p.IsDeleted)
                .Join(context.Areas
                 .Where(a => !a.IsDeleted),
                    processo => processo.AreaId,
                    area => area.Id,
                    (processo, area) => new { Processo = processo, AreaNome = area.Nome })
                .ToListAsync(ct);
            
            var processosIds = processos.Select(x => x.Processo.Id).ToList();
            var detalhes = await context.ProcessosDetalhes
                .Include(processoDetalhe => processoDetalhe.Responsaveis)
                .Include(processoDetalhe => processoDetalhe.Ferramentas)
                .Include(processoDetalhe => processoDetalhe.Documentos)
                .Where(pd => processosIds.Contains(pd.ProcessoId) && !pd.IsDeleted)
                .ToListAsync(ct);
            
            return processos.Select(p => {
                var detalhe = detalhes.FirstOrDefault(d => d.ProcessoId == p.Processo.Id);
                return new ProcessoComDetalhesDto(
                    p.Processo.Id,
                    p.Processo.Nome,
                    p.Processo.Descricao,
                    p.Processo.AreaId,
                    p.AreaNome,
                    p.Processo.ProcessoPaiId,
                    p.Processo.Tipo,
                    p.Processo.Status,
                    detalhe?.Id,
                    detalhe?.Ferramentas.Select(f => new FerramentaProcessoDto(
                        f.Id, f.Nome)).ToList() ?? new List<FerramentaProcessoDto>(),
                    detalhe?.Responsaveis.Select(r => new ResponsavelProcessoDto(
                        r.Id, r.Nome)).ToList() ?? new List<ResponsavelProcessoDto>(),
                    detalhe?.Documentos.Select(d => new DocumentoProcessoDto(
                        d.Id, d.Nome)).ToList() ?? new List<DocumentoProcessoDto>()
                );
            }).ToList();
        });

        rotasProcessos.MapGet("{id}", async (Guid id, AppDbContext context, CancellationToken ct) =>
        {
            var processo = await context.Processos
                .Include(p => p.Area)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

            if (processo == null)
                return Results.NotFound("Processo não encontrado");
            
            var detalhe = await context.ProcessosDetalhes
                .Include(processoDetalhe => processoDetalhe.Responsaveis)
                .Include(processoDetalhe => processoDetalhe.Ferramentas)
                .Include(processoDetalhe => processoDetalhe.Documentos)
                .FirstOrDefaultAsync(pd => pd.ProcessoId == id && !pd.IsDeleted, ct);

            return Results.Ok(new ProcessoComDetalhesDto(
                processo.Id,
                processo.Nome,
                processo.Descricao,
                processo.AreaId,
                processo.Area.Nome,
                processo.ProcessoPaiId,
                processo.Tipo,
                processo.Status,
                detalhe?.Id,
                detalhe?.Ferramentas.Select(f => new FerramentaProcessoDto(
                    f.Id, f.Nome)).ToList() ?? new List<FerramentaProcessoDto>(),
                detalhe?.Responsaveis.Select(r => new ResponsavelProcessoDto(
                    r.Id, r.Nome)).ToList() ?? new List<ResponsavelProcessoDto>(),
                detalhe?.Documentos.Select(d => new DocumentoProcessoDto(
                    d.Id, d.Nome)).ToList() ?? new List<DocumentoProcessoDto>()
            ));
        });

        rotasProcessos.MapGet("{id}/subprocessos", async (Guid id, AppDbContext context, CancellationToken ct) =>
        {
            var processoExiste = await context.Processos
                .AnyAsync(p => p.Id == id && !p.IsDeleted, ct);

            if (!processoExiste)
                return Results.NotFound("Processo não encontrado");

            var subprocessos = await context.Processos
                .Where(p => p.ProcessoPaiId == id && !p.IsDeleted)
                .Select(processo => new ProcessoDto(
                    processo.Id,
                    processo.Nome,
                    processo.Descricao,
                    processo.AreaId,
                    processo.ProcessoPaiId,
                    processo.Tipo,
                    processo.Status
                ))
                .ToListAsync(ct);

            return Results.Ok(subprocessos);
        });

        rotasProcessos.MapPut("{id}",
            async (Guid id, UpdateProcessoRequest request, AppDbContext context, CancellationToken ct) =>
            {
                var processo = await context.Processos.SingleOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

                if (processo == null)
                    return Results.NotFound();
                
                var jaExiste = await context.Processos
                    .Where(p => !p.IsDeleted && p.Id != id)
                    .AnyAsync(p => p.Nome == request.Nome && p.AreaId == processo.AreaId, ct);

                if (jaExiste)
                    return Results.Conflict("Já existe um processo com este nome");

                processo.AlterarNome(request.Nome);
                processo.AlterarDescricao(request.Descricao);
                processo.AlterarTipo(request.Tipo);
                processo.AlterarStatus(request.Status);

                if (request.ProcessoPaiId != processo.ProcessoPaiId)
                {
                    if (request.ProcessoPaiId.HasValue)
                    {
                        var processoPaiExiste = await context.Processos
                            .AnyAsync(p => p.Id == request.ProcessoPaiId && !p.IsDeleted, ct);

                        if (!processoPaiExiste)
                            return Results.NotFound("Processo pai não encontrado");
                    }

                    if (request.ProcessoPaiId == id)
                        return Results.BadRequest("Um processo não pode ser pai de si mesmo");

                    processo.AlterarProcessoPai(request.ProcessoPaiId);
                }

                await context.SaveChangesAsync(ct);
                return Results.Ok(new ProcessoDto(
                    processo.Id,
                    processo.Nome,
                    processo.Descricao,
                    processo.AreaId,
                    processo.ProcessoPaiId,
                    processo.Tipo,
                    processo.Status
                ));
            });

        rotasProcessos.MapDelete("{id}", async (Guid id, AppDbContext context, CancellationToken ct) =>
        {
            var processo = await context.Processos
                .SingleOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

            if (processo == null)
                return Results.NotFound();

            processo.Desativar();

            //Desativação em cascata
            var processoIds = new Queue<Guid>();
            processoIds.Enqueue(id);

            while (processoIds.Count > 0)
            {
                var currentId = processoIds.Dequeue();

                var subprocessos = await context.Processos
                    .Where(p => p.ProcessoPaiId == currentId && !p.IsDeleted)
                    .ToListAsync(ct);

                foreach (var subprocesso in subprocessos)
                {
                    subprocesso.Desativar();

                    processoIds.Enqueue(subprocesso.Id);
                }
            }

            await context.SaveChangesAsync(ct);

            return Results.Ok(new ProcessoDto(
                processo.Id,
                processo.Nome,
                processo.Descricao,
                processo.AreaId,
                processo.ProcessoPaiId,
                processo.Tipo,
                processo.Status));

        });

        // Endpoint para obter a hierarquia de processos (para visualização gráfica)
        rotasProcessos.MapGet("hierarquia", async (AppDbContext context, CancellationToken ct) =>
        {
            var todosProcessos = await context.Processos
                .Where(p => !p.IsDeleted)
                .Include(p => p.Area)
                .ToListAsync(ct);
            
            // Agrupar processos por ID para acesso rápido
            var processosMap = todosProcessos.ToDictionary(p => p.Id);
            
            // Identificar processos raiz (que não têm pai ou cujos pais não existem/estão excluídos)
            var processosRaiz = todosProcessos
                .Where(p => !p.ProcessoPaiId.HasValue || 
                        !processosMap.ContainsKey(p.ProcessoPaiId.Value))
                .ToList();
            
            // Construir a hierarquia
            var resultado = new List<ProcessoHierarquiaDto>();
            foreach (var processoRaiz in processosRaiz)
            {
                var hierarquia = ConstruirHierarquia(processoRaiz, todosProcessos);
                resultado.Add(hierarquia);
            }
            
            return Results.Ok(resultado);
        });

        // Endpoint para obter a hierarquia a partir de um processo específico
        rotasProcessos.MapGet("{id}/hierarquia", async (Guid id, AppDbContext context, CancellationToken ct) =>
        {
            // Verificar se o processo existe
            var processo = await context.Processos
                .Include(p => p.Area)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);
            
            if (processo == null)
                return Results.NotFound("Processo não encontrado");
            
            // Buscar todos os processos não excluídos
            var todosProcessos = await context.Processos
                .Where(p => !p.IsDeleted)
                .Include(p => p.Area)
                .ToListAsync(ct);
            
            // Construir a hierarquia a partir do processo solicitado
            var hierarquia = ConstruirHierarquia(processo, todosProcessos);
            
            return Results.Ok(hierarquia);
        });
    }

    // Método auxiliar para construir a hierarquia recursivamente
    private static ProcessoHierarquiaDto ConstruirHierarquia(Processo processo, List<Processo> todosProcessos)
    {
        var dto = new ProcessoHierarquiaDto
        {
            Id = processo.Id,
            Nome = processo.Nome,
            Tipo = processo.Tipo,
            Status = processo.Status,
            Area = processo.Area?.Nome ?? string.Empty
        };
        
        // Encontrar todos os filhos diretos do processo atual
        var filhos = todosProcessos.Where(p => p.ProcessoPaiId == processo.Id).ToList();
        
        // Recursivamente adicionar os subprocessos
        foreach (var filho in filhos)
        {
            dto.SubProcessos.Add(ConstruirHierarquia(filho, todosProcessos));
        }
        
        return dto;
    }
}