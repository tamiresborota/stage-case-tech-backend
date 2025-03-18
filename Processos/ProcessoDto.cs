namespace CaseStage.Processos;

public record ProcessoDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public Guid AreaId { get; set; }
        public Guid? ProcessoPaiId { get; set; }
        public TipoProcesso Tipo { get; set; }
        public StatusProcesso Status { get; set; }
    
        public ProcessoDto(Guid id, string nome, string descricao, Guid areaId, 
            Guid? processoPaiId, TipoProcesso tipo, StatusProcesso status)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            AreaId = areaId;
            ProcessoPaiId = processoPaiId;
            Tipo = tipo;
            Status = status;
        }
    } 