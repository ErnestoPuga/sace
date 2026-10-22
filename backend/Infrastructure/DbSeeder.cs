using Microsoft.EntityFrameworkCore;
using Sace.Api.Domain;
using Sace.Api.Services;

namespace Sace.Api.Infrastructure;

public static class DbSeeder {
  private sealed record TypeSeed(string Code, string Name, string Description, CorrectionResponsible Responsible);
  public static async Task SeedAsync(IServiceProvider services) {
    using var scope = services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<SaceDbContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var passwords = scope.ServiceProvider.GetRequiredService<IPasswordService>();
    if (configuration.GetValue<bool>("PilotUser:Enabled")) {
      var email = configuration["PilotUser:Email"]?.Trim().ToLowerInvariant();
      var password = configuration["PilotUser:Password"];
      var displayName = configuration["PilotUser:DisplayName"]?.Trim();
      if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(displayName))
        throw new InvalidOperationException("PilotUser está habilitado, pero faltan Email, Password o DisplayName.");
      if (!await db.Users.AnyAsync(x => x.Email == email)) {
        var pilot = new User { Name = displayName, Email = email, PasswordHash = passwords.Hash(password) };
        db.Users.Add(pilot);
        db.SystemAuditLogs.Add(new SystemAuditLog { UserId = pilot.Id, Action = "PILOT_USER_CREATED", EntityType = "User", EntityId = pilot.Id.ToString(), Details = "Usuario piloto creado desde configuración segura." });
        await db.SaveChangesAsync();
      }
    }

    var definitions = new[] {
      new TypeSeed("PEDIMENTO", "Pedimento", "Documento de pedimento; su obligatoriedad depende exclusivamente de reglas configuradas.", CorrectionResponsible.CustomsBroker),
      new TypeSeed("INVOICE", "Factura comercial (legado DEMO)", "Tipo conservado para compatibilidad con expedientes DEMO existentes.", CorrectionResponsible.Company),
      new TypeSeed("COMMERCIAL_INVOICE", "Factura comercial", "Factura comercial del expediente.", CorrectionResponsible.Company),
      new TypeSeed("PACKING_LIST", "Lista de empaque / Packing List", "Relación descriptiva de mercancías y embalajes.", CorrectionResponsible.Company),
      new TypeSeed("TRANSPORT_DOCUMENT", "Documento de transporte", "Documento o paquete documental relacionado con transporte y logística.", CorrectionResponsible.CustomsBroker),
      new TypeSeed("COVE_ACK", "COVE / Acuse de Valor", "Documento o acuse relacionado técnicamente mediante referencia COVE.", CorrectionResponsible.Company),
      new TypeSeed("DODA", "Documento de Operación para Despacho Aduanero", "Documento DODA del expediente.", CorrectionResponsible.CustomsBroker),
      new TypeSeed("ARTICLE_36A_DOCUMENT", "Documento / declaración relacionada con artículo 36-A", "Clasificación documental descriptiva; no implica validación jurídica.", CorrectionResponsible.Company),
      new TypeSeed("INCREMENTABLES_DECLARATION", "Declaración de incrementables", "Declaración aportada al expediente.", CorrectionResponsible.Company),
      new TypeSeed("CUSTOMS_BROKER_INSTRUCTION", "Carta de encomienda / instrucciones al agente aduanal", "Carta o instrucciones operativas al agente aduanal.", CorrectionResponsible.Company),
      new TypeSeed("CARTA_318", "Carta 3.1.8 / Declaración de valor de mercancías", "Evidencia documental clasificatoria; no sustituye universalmente a la factura ni implica una conclusión legal.", CorrectionResponsible.Company),
      new TypeSeed("DECLARACION_INCREMENTABLES", "Declaración de incrementables", "Declaración documental de conceptos incrementables.", CorrectionResponsible.Company),
      new TypeSeed("CARTA_ENCOMIENDA", "Carta de encomienda / instrucciones al agente aduanal", "Instrucciones operativas aportadas al expediente.", CorrectionResponsible.Company),
      new TypeSeed("MANIFESTACION_VALOR_MV_MVE", "Hoja de cálculo / MV - MVE", "Manifestación de Valor u hoja de cálculo según la estructura realmente aportada.", CorrectionResponsible.Company),
      new TypeSeed("TRANSPORT_SERVICE_INVOICE", "Factura de transporte / flete", "Factura por servicios de transporte o flete.", CorrectionResponsible.Company),
      new TypeSeed("INSURANCE_POLICY", "Póliza de seguro", "Póliza aportada como evidencia de cobertura y prima atribuible.", CorrectionResponsible.Company),
      new TypeSeed("INSURANCE_INVOICE", "Factura de seguro", "Factura por servicios o prima de seguro.", CorrectionResponsible.Company),
      new TypeSeed("INSURANCE_PAYMENT", "Comprobante de pago de seguro", "Evidencia de pago relacionada con seguro.", CorrectionResponsible.Company),
      new TypeSeed("INCREMENTABLE_INVOICE", "Factura de incrementables", "Factura de gastos potencialmente incrementables, sujeta a clasificación y revisión.", CorrectionResponsible.Company),
      new TypeSeed("INCREMENTABLE_PAYMENT", "Comprobante de pago de incrementables", "Evidencia de pago relacionada con un gasto documentado.", CorrectionResponsible.Company),
      new TypeSeed("PURCHASE_PAYMENT", "Comprobante de pago de mercancía", "Evidencia de pago de la mercancía.", CorrectionResponsible.Company),
      new TypeSeed("EDOCUMENT_ACK", "Acuse e-Document / VUCEM", "Acuse asociado técnicamente mediante referencia e-Document.", CorrectionResponsible.CustomsBroker),
      new TypeSeed("CUSTOMS_BROKER_INVOICE", "CFDI / Cuenta de gastos del agente aduanal", "CFDI o cuenta de gastos incorporada al expediente.", CorrectionResponsible.CustomsBroker),
      new TypeSeed("IMMEX_DOCUMENT", "Documento IMMEX", "Documento IMMEX capturado; no implica que la operación sea IMMEX.", CorrectionResponsible.Company),
      new TypeSeed("OTHER", "Otro documento", "Documento sin clasificación automática confiable.", CorrectionResponsible.Company),
      new TypeSeed("ORIGIN_CERT", "Certificado de origen", "Certificado usado por la regla exclusivamente DEMO de trato preferencial.", CorrectionResponsible.Company),
      new TypeSeed("IMMEX_DOC", "Documento IMMEX demo", "Tipo legado conservado para la regla DEMO IMMEX.", CorrectionResponsible.Company),
      new TypeSeed("NOM_EVIDENCE", "Evidencia NOM", "Evidencia documental regulatoria; su clasificación no acredita cumplimiento.", CorrectionResponsible.Company),
      new TypeSeed("PERMISO_PREVIO", "Permiso previo", "Documento clasificado como permiso previo; sujeto a revisión.", CorrectionResponsible.Company),
      new TypeSeed("AVISO_AUTOMATICO", "Aviso automático", "Documento clasificado como aviso automático; sujeto a revisión.", CorrectionResponsible.Company),
      new TypeSeed("COFEPRIS_DOCUMENT", "Documento COFEPRIS", "Evidencia documental asociada a COFEPRIS; sujeto a revisión.", CorrectionResponsible.Company),
      new TypeSeed("SEMARNAT_DOCUMENT", "Documento SEMARNAT", "Evidencia documental asociada a SEMARNAT; sujeto a revisión.", CorrectionResponsible.Company),
      new TypeSeed("SEDENA_DOCUMENT", "Documento SEDENA", "Evidencia documental asociada a SEDENA; sujeto a revisión.", CorrectionResponsible.Company),
      new TypeSeed("FITOSANITARIO_CERTIFICATE", "Certificado fitosanitario", "Evidencia documental fitosanitaria; sujeto a revisión.", CorrectionResponsible.Company),
      new TypeSeed("RRNA_OTHER", "Otro documento RRNA", "Otra evidencia documental regulatoria pendiente de revisión.", CorrectionResponsible.Company)
    };
    var existingCodes = await db.DocumentTypes.Select(x => x.Code).ToHashSetAsync(StringComparer.OrdinalIgnoreCase);
    foreach (var item in definitions.Where(x => !existingCodes.Contains(x.Code))) db.DocumentTypes.Add(new DocumentType { Code = item.Code, Name = item.Name, Description = item.Description, CorrectionResponsible = item.Responsible });
    var legacy36A=await db.DocumentTypes.FirstOrDefaultAsync(x=>x.Code=="ARTICLE_36A_DOCUMENT"); if(legacy36A is not null)legacy36A.Name="Carta 3.1.8 / Declaración de valor de mercancías (legado)";
    var legacyIncrementables=await db.DocumentTypes.FirstOrDefaultAsync(x=>x.Code=="INCREMENTABLES_DECLARATION"); if(legacyIncrementables is not null)legacyIncrementables.Name="Declaración de incrementables (legado)";
    var legacyEncomienda=await db.DocumentTypes.FirstOrDefaultAsync(x=>x.Code=="CUSTOMS_BROKER_INSTRUCTION"); if(legacyEncomienda is not null)legacyEncomienda.Name="Carta de encomienda / instrucciones al agente aduanal (legado)";

    var effective = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    var rules = new[] {
      Demo("DEMO-001", "Pedimento requerido", "Todas las operaciones requieren pedimento en esta demostración."), Demo("DEMO-002", "Factura comercial requerida", "Todas las operaciones requieren factura en esta demostración."),
      Demo("DEMO-003", "Certificado de origen para trato preferencial", "Regla DEMO para operaciones con trato preferencial."), Demo("DEMO-004", "Documento IMMEX demostrativo", "Regla DEMO para operaciones marcadas manualmente como IMMEX."), Demo("DEMO-005", "Documento presente con error", "Genera hallazgo DEMO cuando la última versión es inválida."),
      Tech("TECH-001", "Archivo vacío", "Control técnico: rechaza archivos sin contenido."), Tech("TECH-002", "Formato no soportado", "Control técnico: limita los formatos admitidos."), Tech("TECH-003", "XML malformado", "Control técnico: comprueba estructura XML."), Tech("TECH-004", "Archivo duplicado", "Control técnico: compara SHA-256 dentro de una operación."), Tech("TECH-005", "Referencia e-Document inconsistente", "Control técnico: busca documentos con referencias e-Document coincidentes.")
    };
    var legalRules = new[] {
      Legal("LEGAL-LA36-001", "Pedimento electrónico identificable", NormativeSource.CustomsLaw, "Ley Aduanera — texto con última reforma DOF 19/11/2025", "36", null, null, null, AutomationLevel.SEMIAUTOMATIC, true, "Comprueba existencia y coincidencia del número de pedimento."),
      Legal("LEGAL-LA36A-VAL-001", "Información relativa al valor y comercialización", NormativeSource.CustomsLaw, "Ley Aduanera — texto con última reforma DOF 19/11/2025", "36-A", "I", "a", null, AutomationLevel.SEMIAUTOMATIC, false, "Comprueba evidencia de factura, documento equivalente o COVE."),
      Legal("LEGAL-RGCE-318-001", "Obligación de CFDI o documento equivalente por valor comercial", NormativeSource.RGCE, "RGCE para 2026 — publicación DOF 27/12/2025", null, null, null, "3.1.8", AutomationLevel.AUTOMATIC, false, "Aplica el umbral de USD 300 con la información estructurada disponible.", 1, effective, new DateTime(2026, 5, 13, 23, 59, 59, DateTimeKind.Utc)),
      Legal("LEGAL-RGCE-318-001", "Obligación de CFDI o documento equivalente por valor comercial", NormativeSource.RGCE, "Primera Resolución de Modificaciones a las RGCE 2026 — publicación DOF 14/05/2026", null, null, null, "3.1.8", AutomationLevel.AUTOMATIC, false, "Corpus consolidado; conserva el criterio automatizado v1 del umbral de USD 300.", 2, new DateTime(2026, 5, 14, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 5, 14, 0, 0, 0, DateTimeKind.Utc)),
      Legal("LEGAL-RGCE-318-002", "Datos mínimos del documento equivalente", NormativeSource.RGCE, "RGCE para 2026 — publicación DOF 27/12/2025", null, null, null, "3.1.8", AutomationLevel.SEMIAUTOMATIC, true, "Revisa los campos mínimos extraíbles de la factura o documento equivalente.", 1, effective, new DateTime(2026, 5, 13, 23, 59, 59, DateTimeKind.Utc)),
      Legal("LEGAL-RGCE-318-002", "Datos mínimos del documento equivalente", NormativeSource.RGCE, "Primera Resolución de Modificaciones a las RGCE 2026 — publicación DOF 14/05/2026", null, null, null, "3.1.8", AutomationLevel.SEMIAUTOMATIC, true, "Corpus consolidado; conserva el criterio semiautomático v1.", 2, new DateTime(2026, 5, 14, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 5, 14, 0, 0, 0, DateTimeKind.Utc)),
      Legal("LEGAL-ANEXO22-INV-001", "Cotejo factura contra operación", NormativeSource.Appendix22, "Anexo 22 de las RGCE 2026", null, null, null, null, AutomationLevel.SEMIAUTOMATIC, false, "Coteja número, moneda y valor de factura contra el expediente.", 1, new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 19, 23, 59, 59, DateTimeKind.Utc), new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc), "Anexo 22", "Bloque factura/CFDI/documento equivalente"),
      Legal("LEGAL-ANEXO22-INV-001", "Cotejo factura contra operación", NormativeSource.Appendix22, "Primera modificación al Anexo 22 de las RGCE 2026", null, null, null, null, AutomationLevel.SEMIAUTOMATIC, false, "Coteja número, moneda y valor de factura contra el expediente con la modificación vigente.", 2, new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc), "Anexo 22", "Bloque factura/CFDI/documento equivalente"),
      Legal("LEGAL-RGCE-1916-001", "Consistencia del número de acuse de valor", NormativeSource.RGCE, "RGCE para 2026 — publicación DOF 27/12/2025", null, null, null, "1.9.16", AutomationLevel.AUTOMATIC, false, "Compara el COVE de la operación con referencias documentales.", 1, effective, new DateTime(2026, 5, 13, 23, 59, 59, DateTimeKind.Utc)),
      Legal("LEGAL-RGCE-1916-001", "Consistencia del número de acuse de valor", NormativeSource.RGCE, "Primera Resolución de Modificaciones a las RGCE 2026 — publicación DOF 14/05/2026", null, null, null, "1.9.16", AutomationLevel.AUTOMATIC, false, "Corpus consolidado; conserva el cotejo de referencias v1.", 2, new DateTime(2026, 5, 14, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 5, 14, 0, 0, 0, DateTimeKind.Utc)),
      Legal("LEGAL-LA36A-TRANSPORT-001", "Información/documentación de transporte", NormativeSource.CustomsLaw, "Ley Aduanera — texto con última reforma DOF 19/11/2025", "36-A", "I", "b", null, AutomationLevel.SEMIAUTOMATIC, true, "Localiza evidencia de transporte y remite la pertinencia del tipo a revisión."),
      Legal("LEGAL-LA65-001", "Cotejo aritmético del valor en aduana e incrementables", NormativeSource.CustomsLaw, "Ley Aduanera — texto con última reforma DOF 19/11/2025", "65", null, null, null, AutomationLevel.SEMIAUTOMATIC, false, "Calcula valor comercial más incrementables confirmados y coteja el valor declarado."),
      Legal("LEGAL-ORIGIN-001", "Origen preferencial", NormativeSource.TradeAgreement, "Tratado o acuerdo comercial declarado", null, null, null, null, AutomationLevel.MANUAL, true, "Solicita revisión material de la prueba de origen cuando se declara preferencia."),
      Legal("LEGAL-EDOC-001", "Consistencia documental de e-Documents declarados", NormativeSource.RGCE, "RGCE para 2026 — publicación DOF 27/12/2025", null, null, null, null, AutomationLevel.AUTOMATIC, false, "Compara referencias e-Document entre documentos relacionados."),
      Legal("LEGAL-RRNA-001", "Regulaciones y restricciones no arancelarias", NormativeSource.CustomsLaw, "Ley Aduanera — texto con última reforma DOF 19/11/2025 y disposiciones sectoriales", null, null, null, null, AutomationLevel.MANUAL, true, "Presenta datos y evidencia para una determinación humana de RRNA.")
    };
    var existingRules = await db.NormativeRules.ToListAsync();
    foreach (var definition in rules.Concat(legalRules)) {
      var current = existingRules.FirstOrDefault(x => x.Code == definition.Code && x.Version == definition.Version);
      if (current is null) db.NormativeRules.Add(definition);
      else if (definition.IsLegalRule && current.ConfigurationJson.Contains("SACE-LEGAL-v1", StringComparison.Ordinal)) {
        current.Name=definition.Name; current.Description=definition.Description; current.NormativeSource=definition.NormativeSource; current.RuleCategory=definition.RuleCategory; current.SourceDocument=definition.SourceDocument; current.Article=definition.Article; current.Section=definition.Section; current.Subsection=definition.Subsection; current.RuleNumber=definition.RuleNumber; current.Annex=definition.Annex; current.Appendix=definition.Appendix; current.PublicationDate=definition.PublicationDate; current.EffectiveFrom=definition.EffectiveFrom; current.EffectiveTo=definition.EffectiveTo; current.AutomationLevel=definition.AutomationLevel; current.RequiresManualReview=definition.RequiresManualReview; current.IsLegalRule=true; current.IsIncludedInLegalCompliance=true; current.ConfigurationJson=definition.ConfigurationJson;
      }
    }
    await db.SaveChangesAsync();

    var crossRules = new[] {
      Cross("CROSS-INV-PED-INVOICE-001","Factura ↔ pedimento: número de factura","Identificación","COMMERCIAL_INVOICE",DocumentFieldCodes.InvoiceNumber,"PEDIMENTO",DocumentFieldCodes.InvoiceNumber,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-INV-PED-QTY-001","Factura ↔ pedimento: cantidad","Cantidades","COMMERCIAL_INVOICE",DocumentFieldCodes.Quantity,"PEDIMENTO",DocumentFieldCodes.Quantity,CrossComparisonType.EXACT_NUMERIC),
      Cross("CROSS-INV-PED-UNIT-001","Factura ↔ pedimento: unidad","Cantidades","COMMERCIAL_INVOICE",DocumentFieldCodes.Unit,"PEDIMENTO",DocumentFieldCodes.Unit,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-INV-PED-VALUE-001","Factura ↔ pedimento: valor comercial","Valores","COMMERCIAL_INVOICE",DocumentFieldCodes.TotalAmount,"PEDIMENTO",DocumentFieldCodes.CommercialValue,CrossComparisonType.CURRENCY_AMOUNT),
      Cross("CROSS-INV-PED-CURRENCY-001","Factura ↔ pedimento: moneda","Valores","COMMERCIAL_INVOICE",DocumentFieldCodes.Currency,"PEDIMENTO",DocumentFieldCodes.Currency,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-INV-PED-INCOTERM-001","Factura ↔ pedimento: Incoterm","Valores","COMMERCIAL_INVOICE",DocumentFieldCodes.Incoterm,"PEDIMENTO",DocumentFieldCodes.Incoterm,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-INV-COVE-INVOICE-001","Factura ↔ COVE: número de factura","Identificación","COMMERCIAL_INVOICE",DocumentFieldCodes.InvoiceNumber,"COVE_ACK",DocumentFieldCodes.InvoiceNumber,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-INV-COVE-DESC-001","Factura ↔ COVE: descripción","Otros","COMMERCIAL_INVOICE",DocumentFieldCodes.Description,"COVE_ACK",DocumentFieldCodes.Description,CrossComparisonType.NORMALIZED_TEXT),
      Cross("CROSS-INV-COVE-QTY-001","Factura ↔ COVE: cantidad","Cantidades","COMMERCIAL_INVOICE",DocumentFieldCodes.Quantity,"COVE_ACK",DocumentFieldCodes.Quantity,CrossComparisonType.EXACT_NUMERIC),
      Cross("CROSS-INV-COVE-UNIT-001","Factura ↔ COVE: unidad","Cantidades","COMMERCIAL_INVOICE",DocumentFieldCodes.Unit,"COVE_ACK",DocumentFieldCodes.Unit,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-INV-COVE-VALUE-001","Factura ↔ COVE: importe","Valores","COMMERCIAL_INVOICE",DocumentFieldCodes.TotalAmount,"COVE_ACK",DocumentFieldCodes.TotalAmount,CrossComparisonType.CURRENCY_AMOUNT),
      Cross("CROSS-INV-COVE-CURRENCY-001","Factura ↔ COVE: moneda","Valores","COMMERCIAL_INVOICE",DocumentFieldCodes.Currency,"COVE_ACK",DocumentFieldCodes.Currency,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-PL-PED-DESC-001","Packing List ↔ pedimento: descripción","Otros","PACKING_LIST",DocumentFieldCodes.Description,"PEDIMENTO",DocumentFieldCodes.Description,CrossComparisonType.NORMALIZED_TEXT),
      Cross("CROSS-PL-PED-QTY-001","Packing List ↔ pedimento: cantidad","Cantidades","PACKING_LIST",DocumentFieldCodes.Quantity,"PEDIMENTO",DocumentFieldCodes.Quantity,CrossComparisonType.EXACT_NUMERIC),
      Cross("CROSS-PL-PED-UNIT-001","Packing List ↔ pedimento: unidad","Cantidades","PACKING_LIST",DocumentFieldCodes.Unit,"PEDIMENTO",DocumentFieldCodes.Unit,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-PL-PED-WEIGHT-001","Packing List ↔ pedimento: peso bruto","Transporte","PACKING_LIST",DocumentFieldCodes.GrossWeight,"PEDIMENTO",DocumentFieldCodes.GrossWeight,CrossComparisonType.PERCENT_TOLERANCE,percent:2m),
      Cross("CROSS-PL-TR-PACKAGES-001","Packing List ↔ transporte: bultos","Transporte","PACKING_LIST",DocumentFieldCodes.PackageCount,"TRANSPORT_DOCUMENT",DocumentFieldCodes.PackageCount,CrossComparisonType.EXACT_NUMERIC),
      Cross("CROSS-PL-TR-WEIGHT-001","Packing List ↔ transporte: peso bruto","Transporte","PACKING_LIST",DocumentFieldCodes.GrossWeight,"TRANSPORT_DOCUMENT",DocumentFieldCodes.GrossWeight,CrossComparisonType.PERCENT_TOLERANCE,percent:2m),
      Cross("CROSS-FREIGHT-INV-PED-001","Factura de flete ↔ pedimento","Valoración","TRANSPORT_SERVICE_INVOICE",DocumentFieldCodes.FreightAmount,"PEDIMENTO",DocumentFieldCodes.DeclaredFreight,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-FREIGHT-PAY-INV-001","Pago de flete ↔ factura de flete","Valoración","INCREMENTABLE_PAYMENT",DocumentFieldCodes.PaymentAmount,"TRANSPORT_SERVICE_INVOICE",DocumentFieldCodes.FreightAmount,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-FREIGHT-TR-INV-001","Referencia transporte ↔ factura de flete","Valoración","TRANSPORT_DOCUMENT",DocumentFieldCodes.TransportReference,"TRANSPORT_SERVICE_INVOICE",DocumentFieldCodes.TransportReference,CrossComparisonType.NORMALIZED_IDENTIFIER),
      Cross("CROSS-INS-POL-PED-001","Póliza de seguro ↔ pedimento","Valoración","INSURANCE_POLICY",DocumentFieldCodes.InsuranceAmount,"PEDIMENTO",DocumentFieldCodes.DeclaredInsurance,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-INS-INV-PED-001","Factura de seguro ↔ pedimento","Valoración","INSURANCE_INVOICE",DocumentFieldCodes.InsuranceAmount,"PEDIMENTO",DocumentFieldCodes.DeclaredInsurance,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-INS-PAY-INV-001","Pago de seguro ↔ factura de seguro","Valoración","INSURANCE_PAYMENT",DocumentFieldCodes.PaymentAmount,"INSURANCE_INVOICE",DocumentFieldCodes.InsuranceAmount,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-INC-INV-PED-001","Total factura incrementable ↔ pedimento","Valoración","INCREMENTABLE_INVOICE",DocumentFieldCodes.IncrementableTotal,"PEDIMENTO",DocumentFieldCodes.IncrementableTotal,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-INC-PAY-INV-001","Pago incrementable ↔ factura incrementable","Valoración","INCREMENTABLE_PAYMENT",DocumentFieldCodes.PaymentAmount,"INCREMENTABLE_INVOICE",DocumentFieldCodes.IncrementableTotal,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-VAL-MV-PED-001","MV/MVE ↔ valor en aduana declarado","Valoración","MANIFESTACION_VALOR_MV_MVE",DocumentFieldCodes.DeclaredCustomsValue,"PEDIMENTO",DocumentFieldCodes.DeclaredCustomsValue,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-VAL-COMMERCIAL-001","Factura comercial ↔ valor comercial declarado","Valoración","COMMERCIAL_INVOICE",DocumentFieldCodes.TotalAmount,"PEDIMENTO",DocumentFieldCodes.CommercialValue,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m),
      Cross("CROSS-VAL-COVE-001","COVE ↔ valor comercial soportado","Valoración","COVE_ACK",DocumentFieldCodes.TotalAmount,"COMMERCIAL_INVOICE",DocumentFieldCodes.TotalAmount,CrossComparisonType.CURRENCY_AMOUNT,absolute:.01m)
    };
    var existingCross=await db.CrossValidationRules.Select(x=>new{x.Code,x.Version}).ToListAsync(); foreach(var item in crossRules.Where(x=>!existingCross.Any(e=>e.Code==x.Code&&e.Version==x.Version)))db.CrossValidationRules.Add(item); await db.SaveChangesAsync();

    var regulatorySeeds = new[] {
      new RegulatoryRequirement { Code="DEMO-RRNA-PERMISO-001",Version=1,Name="Permiso previo demostrativo",Description="Escenario sintético DRAFT para validar el motor; no representa una regulación real.",RequirementCategory=RequirementCategory.PERMISO_PREVIO,AuthorityCode=RegulatoryAuthorityCode.ECONOMIA,OperationScope=RegulatoryOperationScope.IMPORT,EffectiveFrom=effective,Status=RegulatoryRequirementStatus.DRAFT,IsAutomatable=true,Conditions=[new(){FieldCode=DocumentFieldCodes.TariffFraction,Operator=RegulatoryConditionOperator.EQUALS,ExpectedValue="00000001",ConditionGroup="A",Sequence=1}],EvidenceRequirements=[new(){DocumentTypeCode="PERMISO_PREVIO",EvidenceRole=RegulatoryEvidenceRole.PRIMARY,IsRequired=true,Description="Evidencia sintética esperada."}] },
      new RegulatoryRequirement { Code="DEMO-RRNA-NOM-001",Version=1,Name="Evidencia NOM demostrativa",Description="Escenario sintético DRAFT para revisión manual; no representa una regulación real.",RequirementCategory=RequirementCategory.NOM,AuthorityCode=RegulatoryAuthorityCode.ECONOMIA,OperationScope=RegulatoryOperationScope.BOTH,EffectiveFrom=effective,Status=RegulatoryRequirementStatus.DRAFT,IsAutomatable=false,RequiresManualReview=true,Conditions=[new(){FieldCode=DocumentFieldCodes.Description,Operator=RegulatoryConditionOperator.PRESENT,ConditionGroup="A",Sequence=1}],EvidenceRequirements=[new(){DocumentTypeCode="NOM_EVIDENCE",EvidenceRole=RegulatoryEvidenceRole.PRIMARY,IsRequired=true,Description="Evidencia sintética esperada."}] }
    };
    var existingRegulatory=await db.RegulatoryRequirements.Select(x=>new{x.Code,x.Version}).ToListAsync();
    foreach(var item in regulatorySeeds.Where(x=>!existingRegulatory.Any(e=>e.Code==x.Code&&e.Version==x.Version)))db.RegulatoryRequirements.Add(item);
    await db.SaveChangesAsync();

    if (!configuration.GetValue<bool>("Seed:DemoData")) return;
    var admin = await db.Users.FirstOrDefaultAsync(x => x.Email == "admin@sace.local");
    if (admin is null) { admin = new User { Name = "Administrador SACE", Email = "admin@sace.local", PasswordHash = passwords.Hash("Admin123!") }; db.Users.Add(admin); await db.SaveChangesAsync(); }
    if (await db.TradeOperations.AnyAsync(x => x.Folio == "IMP-2026-000001")) return;
    var docs = await db.DocumentTypes.ToDictionaryAsync(x => x.Code, StringComparer.OrdinalIgnoreCase);
    var countries = new[] { ("Estados Unidos", "México"), ("Alemania", "México"), ("México", "Canadá"), ("China", "México") }; var operations = new List<TradeOperation>();
    for (var i = 1; i <= 12; i++) {
      var import = i <= 7; var date = new DateTime(2026, i <= 6 ? 8 : 9, Math.Min(3 + i, 25), 0, 0, 0, DateTimeKind.Utc); var preferential = i is 1 or 3 or 8; var immex = i is not 5 and not 10;
      var op = new TradeOperation { Folio = $"{(import ? "IMP" : "EXP")}-2026-{i:D6}", PedimentoNumber = $"26 48 1234 6{i:D6}", OperationType = import ? OperationType.Importacion : OperationType.Exportacion, Period = date.ToString("yyyy-MM"), OperationDate = date, PedimentoKey = import ? "IN" : "RT", TariffFraction = $"{85044000 + i}", OriginCountry = countries[(i - 1) % countries.Length].Item1, DestinationCountry = countries[(i - 1) % countries.Length].Item2, CustomsRegime = immex ? "Temporal" : "Definitivo", IsImmex = immex, PreferentialTreatment = preferential, TradeAgreement = preferential ? "T-MEC (dato demostrativo)" : null, Status = OperationStatus.PendingAudit };
      operations.Add(op); db.TradeOperations.Add(op);
    }
    await db.SaveChangesAsync();
    foreach (var op in operations) { AddDemoDoc(db, op, docs["PEDIMENTO"], "pedimento.xml", admin.Id); AddDemoDoc(db, op, docs["INVOICE"], "factura.pdf", admin.Id); if (op.IsImmex) AddDemoDoc(db, op, docs["IMMEX_DOC"], "immex-demo.xlsx", admin.Id); if (op.PreferentialTreatment && op.Folio is not "IMP-2026-000001" and not "IMP-2026-000003") AddDemoDoc(db, op, docs["ORIGIN_CERT"], "certificado-origen.pdf", admin.Id); }
    await db.SaveChangesAsync(); var engine = scope.ServiceProvider.GetRequiredService<IAuditEngine>(); foreach (var op in operations) await engine.ExecuteAsync(op.Id, AuditType.Daily, admin.Id);
    var demoFinding = await db.Findings.FirstOrDefaultAsync(x => x.TradeOperation.Folio == "IMP-2026-000001"); if (demoFinding is not null) { demoFinding.Description = "Certificado de origen no localizado. Regla exclusivamente DEMO."; demoFinding.Severity = Severity.High; }
    db.SystemAuditLogs.Add(new SystemAuditLog { UserId = admin.Id, Action = "DEMO_DATA_SEEDED", EntityType = "System", EntityId = "seed", Details = "Se generaron 12 operaciones y reglas DEMO; no representan validación jurídica." }); await db.SaveChangesAsync();
    NormativeRule Demo(string code, string name, string description) => new() { Code = code, Name = name, Description = description, EffectiveFrom = effective, NormativeSource = NormativeSource.Demo, RuleCategory = RuleCategory.DEMO, IsLegalRule = false, IsIncludedInLegalCompliance = false, ConfigurationJson = "{\"demo\":true}" };
    NormativeRule Tech(string code, string name, string description) => new() { Code = code, Name = name, Description = description, EffectiveFrom = effective, NormativeSource = NormativeSource.Technical, RuleCategory = RuleCategory.TECHNICAL, IsLegalRule = false, IsIncludedInLegalCompliance = false, RuleType = "TechnicalControl", ConfigurationJson = "{\"legalInterpretation\":false}" };
    NormativeRule Legal(string code, string name, NormativeSource source, string sourceDocument, string? article, string? section, string? subsection, string? ruleNumber, AutomationLevel automation, bool manual, string description, int version = 1, DateTime? from = null, DateTime? to = null, DateTime? publication = null, string? annex = null, string? appendix = null) => new() { Code = code, Name = name, Description = description, NormativeSource = source, RuleCategory = RuleCategory.LEGAL, SourceDocument = sourceDocument, Article = article, Section = section, Subsection = subsection, RuleNumber = ruleNumber, Annex = annex, Appendix = appendix, PublicationDate = publication ?? (source == NormativeSource.CustomsLaw ? new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Utc) : new DateTime(2025, 12, 27, 0, 0, 0, DateTimeKind.Utc)), EffectiveFrom = from ?? effective, EffectiveTo = to, Version = version, AutomationLevel = automation, RequiresManualReview = manual, IsLegalRule = true, IsIncludedInLegalCompliance = true, RuleType = "LegalEvaluation", ConfigurationJson = code switch { "LEGAL-RGCE-318-001" => "{\"corpus\":\"SACE-LEGAL-v1\",\"thresholdUsd\":300}", "LEGAL-ANEXO22-INV-001" => "{\"corpus\":\"SACE-LEGAL-v1\",\"amountTolerance\":0.01}", "LEGAL-LA65-001" => "{\"corpus\":\"SACE-LEGAL-v1\",\"toleranceMxn\":1.00}", _ => "{\"corpus\":\"SACE-LEGAL-v1\"}" } };
    CrossValidationRule Cross(string code,string name,string category,string sourceType,string sourceField,string targetType,string targetField,CrossComparisonType comparison,decimal? absolute=null,decimal? percent=null)=>new(){Code=code,Version=1,Name=name,Description="Comparación técnica/documental; no constituye una conclusión jurídica.",Category=category,SourceDocumentTypeCode=sourceType,SourceFieldCode=sourceField,TargetDocumentTypeCode=targetType,TargetFieldCode=targetField,ComparisonType=comparison,AbsoluteTolerance=absolute,PercentTolerance=percent,Severity=Severity.Medium,MissingEvidenceBehavior=MissingEvidenceBehavior.INSUFFICIENT_EVIDENCE,EffectiveFrom=effective};
  }
  private static void AddDemoDoc(SaceDbContext db, TradeOperation op, DocumentType type, string name, Guid userId) => db.OperationDocuments.Add(new OperationDocument { TradeOperationId = op.Id, DocumentTypeId = type.Id, OriginalFileName = name, StoredFileName = $"demo-{Guid.NewGuid():N}{Path.GetExtension(name)}", MimeType = name.EndsWith(".pdf") ? "application/pdf" : name.EndsWith(".xml") ? "application/xml" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Extension = Path.GetExtension(name), FileSize = 1024, FilePath = $"demo/{op.Id}/{name}", Version = 1, ValidationStatus = ValidationStatus.Valid, UploadedBy = userId });
}
