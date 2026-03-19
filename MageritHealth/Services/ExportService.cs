using MageritHealth.Repositories.Interfaces;
using MageritHealth.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MageritHealth.Services
{
    public class ExportService : IExportService
    {
        private readonly IAnaliticasRepository analiticasRepository;
        // 1. Añadir el repositorio de prescripciones
        private readonly IPrescripcionesRepository prescripcionesRepository;
        private readonly ICitasRepository citasRepository;

        // 2. Inyectarlo en el constructor
        public ExportService(IAnaliticasRepository analiticasRepository, IPrescripcionesRepository prescripcionesRepo, ICitasRepository citasRepo)
        {
            this.analiticasRepository = analiticasRepository;
            this.prescripcionesRepository = prescripcionesRepo;
            this.citasRepository = citasRepo;

            // Configuración de licencia requerida por QuestPDF (Community es gratis para pymes/proyectos personales)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerarInformeAnaliticaPdfAsync(int idAnalitica)
        {
            // 1. Obtener los datos (Asegúrate de que tu repo incluya el TipoMedicion y la Cita/Paciente)
            var mediciones = await this.analiticasRepository.GetListaMedicionesByIdAnaliticaAsync(idAnalitica);

            // Si necesitas datos del paciente, idealmente los pasas aquí o los sacas de la primera medición
            var analitica = mediciones.FirstOrDefault()?.Analitica;
            var fecha = analitica?.FechaAnalitica.ToString("dd/MM/yyyy") ?? "Desconocida";

            // 2. Construir el documento
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    // Cabecera
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Magerit Health").FontSize(20).SemiBold().FontColor("#0f4c81");
                            col.Item().Text("Informe de Resultados de Laboratorio").FontSize(14).FontColor(Colors.Grey.Darken2);
                            col.Item().PaddingTop(5).Text($"Fecha de la prueba: {fecha}");
                            // Si tuvieras el paciente: col.Item().Text($"Paciente: {analitica.Cita.Paciente.Nombre}");
                        });

                        // row.ConstantItem(100).Height(50).Placeholder();
                    });

                    // Contenido (La Tabla)
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            // Definir las 4 columnas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Parámetro
                                columns.RelativeColumn(2); // Resultado
                                columns.RelativeColumn(2); // Rango Ref.
                                columns.RelativeColumn(1); // Unidades
                            });

                            // Estilo de la cabecera de la tabla
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Parámetro").SemiBold().FontColor(Colors.White);
                                header.Cell().Element(CellStyle).Text("Resultado").SemiBold().FontColor(Colors.White);
                                header.Cell().Element(CellStyle).Text("Rango Ref.").SemiBold().FontColor(Colors.White);
                                header.Cell().Element(CellStyle).Text("Unidades").SemiBold().FontColor(Colors.White);

                                IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).Background("#0f4c81").PaddingHorizontal(5);
                                }
                            });

                            // Filas con los datos
                            foreach (var med in mediciones)
                            {
                                var tipo = med.TipoMedicion;
                                bool fueraDeRango = med.ValorMedicion < tipo.ValorMinimo || med.ValorMedicion > tipo.ValorMaximo;
                                string colorTexto = fueraDeRango ? Colors.Red.Medium : Colors.Black;

                                table.Cell().Element(BlockStyle).Text(tipo.NombreMedicion);

                                // El resultado se pinta de rojo y en negrita si está mal
                                var textResult = table.Cell().Element(BlockStyle).Text(med.ValorMedicion.ToString("0.00")).FontColor(colorTexto);
                                if (fueraDeRango) textResult.SemiBold();

                                table.Cell().Element(BlockStyle).Text($"{tipo.ValorMinimo} - {tipo.ValorMaximo}");
                                table.Cell().Element(BlockStyle).Text(tipo.UnidadMedicion);

                                IContainer BlockStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(5);
                                }
                            }
                        });

                        col.Item().PaddingTop(25).Text("Observaciones:").SemiBold();
                        col.Item().Text(analitica?.Notas ?? "Sin observaciones.").FontColor(Colors.Grey.Darken3);
                    });

                    // Pie de página
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerarInformeCitaPdfAsync(int idCita)
        {
            var cita = await citasRepository.GetCitaByIdAsync(idCita);

            if (cita == null)
            {
                return null;
            }

            var paciente = cita.Paciente;
            var doctor = cita.Doctor;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial).FontColor(Colors.Black));

                    // --- CABECERA ---
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Magerit Health").FontSize(24).SemiBold().FontColor("#0f4c81");
                            row.ConstantItem(150).AlignRight().Text("INFORME CLÍNICO").FontSize(14).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().PaddingTop(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            // Columna izquierda: Datos del Paciente
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("DATOS DEL PACIENTE").FontSize(9).FontColor(Colors.Grey.Darken1).SemiBold();
                                c.Item().Text($"{paciente.Nombre} {paciente.Apellido1} {paciente.Apellido2}").FontSize(12).SemiBold();
                                c.Item().Text($"DNI: {paciente.Dni}");
                                c.Item().Text($"Nº Asegurado: {paciente.NumeroAsegurado ?? "N/A"}");
                            });

                            // Columna derecha: Datos de la Cita / Doctor
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text("DETALLES DE LA CONSULTA").FontSize(9).FontColor(Colors.Grey.Darken1).SemiBold();
                                c.Item().Text($"Fecha: {cita.FechaHora.ToString("dd/MM/yyyy HH:mm")}").SemiBold();
                                c.Item().Text($"Dr/a. {doctor.Nombre} {doctor.Apellido1}");
                                c.Item().Text($"{doctor.Especialidad?.NombreEspecialidad ?? "Especialista"} (Col: {doctor.NumeroColegiado})").FontSize(10).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    // --- CONTENIDO DEL INFORME ---
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Bloque 1: Motivo de la visita
                        col.Item().PaddingBottom(15).Column(c =>
                        {
                            c.Item().Text("Motivo de la Visita:").FontSize(12).SemiBold().FontColor("#0f4c81");
                            c.Item().PaddingTop(5).Background("#f8f9fa").Padding(10).Text(cita.Motivo);
                        });

                        // Bloque 2: Notas Clínicas (Solo si existen)
                        col.Item().PaddingBottom(15).Column(c =>
                        {
                            c.Item().Text("Juicio Clínico / Observaciones:").FontSize(12).SemiBold().FontColor("#0f4c81");

                            string notas = string.IsNullOrWhiteSpace(cita.Notas)
                                ? "El especialista no ha registrado observaciones adicionales en esta consulta."
                                : cita.Notas;

                            c.Item().PaddingTop(5).Text(notas);
                        });
                    });

                    // --- PIE DE PÁGINA ---
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Text(text =>
                            {
                                // 1. Aplicamos el estilo por defecto PARA ESTE BLOQUE aquí dentro:
                                text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Medium));

                                text.Span("Magerit Health").SemiBold();
                                text.Span(" | Documento generado el " + System.DateTime.Now.ToString("dd/MM/yyyy"));
                            });

                            row.RelativeItem().AlignRight().Text(text =>
                            {
                                // 2. Y lo mismo para el bloque de la paginación:
                                text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Medium));

                                text.Span("Página ");
                                text.CurrentPageNumber();
                                text.Span(" de ");
                                text.TotalPages();
                            });
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerarRecetasPorCitaPdfAsync(int idCita)
        {
            // 1. Obtener los datos (Asegúrate de incluir Medicamento, Cita, Cita.Paciente y Cita.Doctor)
            var prescripciones = await this.prescripcionesRepository.GetListaPrescripcionesByIdCitaAsync(idCita);

            if (prescripciones == null || !prescripciones.Any())
            {
                return null; // O lanza una excepción, según prefieras
            }

            var cita = prescripciones.First().Cita;
            var paciente = cita.Paciente;
            var doctor = cita.Doctor;

            // 2. Construir el documento PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5); // A5 es el tamaño estándar de una receta médica (más pequeño que A4)
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    // --- CABECERA: Logos y Datos Clínicos ---
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Magerit Health").FontSize(18).SemiBold().FontColor("#0f4c81");
                            row.ConstantItem(100).AlignRight().Text("RECETA MÉDICA").FontSize(12).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Datos del Doctor y Fecha
                        col.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Dr./a. {doctor.Nombre} {doctor.Apellido1} {doctor.Apellido2}").SemiBold();
                                c.Item().Text($"Nº Colegiado: {doctor.NumeroColegiado}").FontSize(9).FontColor(Colors.Grey.Darken2);
                                c.Item().Text($"Especialidad: {doctor.Especialidad?.NombreEspecialidad}").FontSize(9).FontColor(Colors.Grey.Darken2);
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text($"Fecha: {cita.FechaHora.ToString("dd/MM/yyyy")}").SemiBold();
                            });
                        });

                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Datos del Paciente
                        col.Item().PaddingTop(5).Column(c =>
                        {
                            c.Item().Text("DATOS DEL PACIENTE").FontSize(8).FontColor(Colors.Grey.Medium).SemiBold();
                            c.Item().Text($"{paciente.Nombre} {paciente.Apellido1} {paciente.Apellido2}").FontSize(11).SemiBold();
                            c.Item().Text($"DNI: {paciente.Dni} | Nº Asegurado: {paciente.NumeroAsegurado ?? "N/A"}").FontSize(9);
                        });

                        col.Item().Height(15);
                    });

                    // --- CONTENIDO: Lista de Medicamentos ---
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Text("PLAN DE MEDICACIÓN").FontSize(12).SemiBold().FontColor("#0f4c81");

                        foreach (var prescripcion in prescripciones)
                        {
                            var med = prescripcion.Medicamento;

                            col.Item().PaddingBottom(15).Background("#f8f9fa").Border(1).BorderColor(Colors.Grey.Lighten3).Padding(10).Column(medCol =>
                            {
                                // Fila 1: Nombre Comercial y Formato
                                medCol.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(text =>
                                    {
                                        text.Span($"{med.NombreComercial} {med.Concentracion} ").FontSize(11).SemiBold().FontColor(Colors.Black);
                                        text.Span($"({med.Formato})").FontSize(9).FontColor(Colors.Grey.Darken2).Italic();
                                    });
                                });

                                // Fila 2: Principio activo
                                medCol.Item().PaddingBottom(5).Text(med.PrincipioActivo).FontSize(8).FontColor(Colors.Grey.Medium);

                                // Fila 3: Instrucciones de toma (Lo más importante)
                                medCol.Item().Text(text =>
                                {
                                    text.Span("Pauta: ").SemiBold();
                                    text.Span(prescripcion.Instrucciones);
                                });

                                // Fila 4: Duración del tratamiento
                                medCol.Item().PaddingTop(5).Text(text =>
                                {
                                    text.Span("Duración: ").FontSize(9).SemiBold();
                                    text.Span($"Del {prescripcion.FechaInicio.ToString("dd/MM/yyyy")} al {prescripcion.FechaFin.ToString("dd/MM/yyyy")}").FontSize(9);
                                });
                            });
                        }
                    });

                    // --- PIE DE PÁGINA: Firmas y Validez ---
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Text("Firma del Facultativo:").FontSize(8).FontColor(Colors.Grey.Darken1);
                        });

                        // Espacio para la firma física
                        col.Item().Height(40);

                        col.Item().AlignCenter().Text("Documento generado electrónicamente por Magerit Health. Válido para dispensación en farmacias.").FontSize(7).FontColor(Colors.Grey.Medium).Italic();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}