using System;
using System.Collections.Generic;
using System.Linq;
using CaseStage.Areas;

namespace CaseStage.Processos;

public class Processo
{
    public Guid Id { get; init; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public TipoProcesso Tipo { get; private set; }
    public StatusProcesso Status { get; private set; }
    public bool IsDeleted { get; set; } = false;
    
    public Guid? ProcessoPaiId { get; private set; }
    public Processo? ProcessoPai { get; private set; }
    public ICollection<Processo> SubProcessos { get; private set; } = new List<Processo>();
    
    public Guid AreaId { get; private set; }
    public Area Area { get; private set; }
    
    public Processo(string nome, string descricao, Guid areaId, TipoProcesso tipo = TipoProcesso.Manual, StatusProcesso status = StatusProcesso.Implementado)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
        AreaId = areaId;
        Tipo = tipo;
        Status = status;
    }
    
    public Processo(string nome, string descricao, Guid areaId, Guid processoPaiId, TipoProcesso tipo = TipoProcesso.Manual, StatusProcesso status = StatusProcesso.Implementado)
        : this(nome, descricao, areaId, tipo, status)
    {
        ProcessoPaiId = processoPaiId;
    }
    
    public void AlterarNome(string nome)
    {
        Nome = nome;
    }
    
    public void AlterarDescricao(string descricao)
    {
        Descricao = descricao;
    }
    
    public void AlterarTipo(TipoProcesso tipo)
    {
        Tipo = tipo;
    }
    
    public void AlterarStatus(StatusProcesso status)
    {
        Status = status;
    }
    
    public void AlterarProcessoPai(Guid? processoPaiId)
    {
        if (processoPaiId == Id)
            throw new InvalidOperationException("Um processo não pode ser pai de si mesmo.");
            
        ProcessoPaiId = processoPaiId;
    }
    
    public void Desativar()
    {
        IsDeleted = true;

    }
    
}

public enum TipoProcesso
{
    Manual,
    Sistemico
}

public enum StatusProcesso
{
    Implementado,
    EmImplementacao,
    Planejado,
    Problematico,
    Obsoleto
}