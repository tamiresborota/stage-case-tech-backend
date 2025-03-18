using CaseStage.Data;
using CaseStage.Processos;
using Microsoft.EntityFrameworkCore;

namespace CaseStage.Areas;

public static class AreasRoutes
{
    public static void AddAreasRoutes(this WebApplication app)
    {
        var rotasAreas = app.MapGroup("areas");
        
        rotasAreas.MapPost("", async (AddAreaRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var jaExisteAtiva = await context.Areas
                .AnyAsync(area => area.Nome == request.Nome && !area.IsDeleted, ct);
    
            if (jaExisteAtiva)
                return Results.Conflict(error: "Já existe uma área ativa com esse nome!");
    
            var areaDeletada = await context.Areas
                .FirstOrDefaultAsync(area => area.Nome == request.Nome && area.IsDeleted, ct);
    
            AreaDto areaRetorno;
    
            if (areaDeletada != null)
            {
                // Reativar a área existente
                areaDeletada.IsDeleted = false;
        
                context.Areas.Update(areaDeletada);
                await context.SaveChangesAsync(ct);
        
                areaRetorno = new AreaDto(areaDeletada.Id, areaDeletada.Nome);
        
                return Results.Ok(areaRetorno);
            }
            else
            {
                // Criar uma nova área se não existir nem mesmo como deletada
                var novaArea = new Area(request.Nome);
                await context.Areas.AddAsync(novaArea);
                await context.SaveChangesAsync(ct);
        
                areaRetorno = new AreaDto(novaArea.Id, novaArea.Nome);
        
                return Results.Ok(areaRetorno);
            }
        });

        rotasAreas.MapGet("", async (AppDbContext context, CancellationToken ct) =>
        {
            var areas = await context.Areas.Where(a => !a.IsDeleted).Select(area => new AreaDto(area.Id, area.Nome)).ToListAsync(ct);
            
            return areas;
        });

        rotasAreas.MapPut("{id}", async (Guid id, UpdateAreaRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var area = await context.Areas.SingleOrDefaultAsync(area => area.Id == id, ct);
            
            if (area == null)
                return Results.NotFound();
            
            var jaExiste = await context.Areas
                .Where(a => !a.IsDeleted && a.Id != id)
                .AnyAsync(a => a.Nome == request.Nome, ct);

            if (jaExiste)
                return Results.Conflict("Já existe um processo com este nome");
              
            area.AlterarName(request.Nome);
            
            await context.SaveChangesAsync(ct);
            return Results.Ok(new AreaDto(area.Id, area.Nome));
            
        });
        
        rotasAreas.MapDelete("{id}", async (Guid id, AppDbContext context, CancellationToken ct) =>
            {
                var area = await context.Areas.SingleOrDefaultAsync(area => area.Id == id, ct);
                
                if (area == null)
                    return Results.NotFound();
                
                area.Desativar();
                await context.SaveChangesAsync(ct);
                return Results.Ok(area);
                
            });

        rotasAreas.MapGet("{areaId}/processos", async (Guid areaId, AppDbContext context, CancellationToken ct) =>
        {
            var areaExiste = await context.Areas
                .AnyAsync(a => a.Id == areaId && !a.IsDeleted, ct);

            if (!areaExiste)
                return Results.NotFound("Área não encontrada");

            var processos = await context.Processos
                .Where(p => p.AreaId == areaId && !p.IsDeleted)
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

            return Results.Ok(processos);
        });

        rotasAreas.MapGet("{id}", async (Guid id, AppDbContext context, CancellationToken ct) =>
        {
            var area = await context.Areas
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct);
                
            if (area == null)
                return Results.NotFound("Área não encontrada");
                
            return Results.Ok(new AreaDto(area.Id, area.Nome));
        });
    }
}