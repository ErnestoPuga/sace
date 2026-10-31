import { useCallback, useEffect, useState } from 'react'
import { CheckCircle2, Plus, RefreshCw } from 'lucide-react'
import { api, messageOf } from '../lib/api'
import { Badge, Empty, PageHeader, Spinner, fmtDate, fmtDateTime, human } from '../components'

const today = () => new Date().toISOString().slice(0, 10)
const number = (value: unknown) => Number(value || 0).toLocaleString('es-MX', { maximumFractionDigits: 6 })

function ErrorNotice({ message }: { message: string }) {
  return message ? <div className="mb-5 rounded-xl bg-rose-50 p-4 text-sm text-rose-700">{message}</div> : null
}

function PageNav({ data, onPage }: { data: any, onPage: (page: number) => void }) {
  if (!data || data.pages <= 1) return null
  return <div className="flex items-center justify-between border-t border-slate-200 px-5 py-4 text-sm">
    <span>Página {data.page} de {data.pages}</span>
    <div className="flex gap-2">
      <button className="btn-secondary" disabled={data.page <= 1} onClick={() => onPage(data.page - 1)}>Anterior</button>
      <button className="btn-secondary" disabled={data.page >= data.pages} onClick={() => onPage(data.page + 1)}>Siguiente</button>
    </div>
  </div>
}

export function ImmexConversionsAndRules() {
  const [materials, setMaterials] = useState<any[]>([])
  const [conversions, setConversions] = useState<any[]>([])
  const [rules, setRules] = useState<any[]>([])
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const [form, setForm] = useState({ materialId: '', fromUnit: '', toUnit: '', factor: '', effectiveFrom: today(), effectiveTo: '', source: '', isVerified: false })

  const load = useCallback(async () => {
    setError('')
    try {
      const [materialResponse, conversionResponse, ruleResponse] = await Promise.all([
        api.get('/immex/materials', { params: { page: 1, pageSize: 100 } }),
        api.get('/immex/unit-conversions'),
        api.get('/immex/temporalities/rules')
      ])
      setMaterials(materialResponse.data.items)
      setConversions(conversionResponse.data)
      setRules(ruleResponse.data)
    } catch (reason) {
      setError(messageOf(reason))
    }
  }, [])

  useEffect(() => { void load() }, [load])

  async function createConversion() {
    setBusy(true)
    setError('')
    try {
      await api.post('/immex/unit-conversions', {
        materialId: form.materialId,
        fromUnit: form.fromUnit,
        toUnit: form.toUnit,
        factor: Number(form.factor),
        effectiveFrom: new Date(`${form.effectiveFrom}T12:00:00Z`).toISOString(),
        effectiveTo: form.effectiveTo ? new Date(`${form.effectiveTo}T12:00:00Z`).toISOString() : null,
        source: form.source,
        isVerified: form.isVerified
      })
      setForm({ materialId: '', fromUnit: '', toUnit: '', factor: '', effectiveFrom: today(), effectiveTo: '', source: '', isVerified: false })
      await load()
    } catch (reason) {
      setError(messageOf(reason))
    } finally {
      setBusy(false)
    }
  }

  const materialName = (id: string) => materials.find(x => x.id === id)?.materialCode || 'Material no disponible'

  return <>
    <PageHeader eyebrow="Administración IMMEX" title="Conversiones y temporalidades" description="Las conversiones requieren un factor capturado con su fuente. Las reglas VERIFIED son versionadas y de solo lectura." action={<button className="btn-secondary" onClick={() => void load()}><RefreshCw size={17}/>Actualizar</button>}/>
    <ErrorNotice message={error}/>
    <section className="card mb-6 p-5">
      <h3 className="font-bold">Registrar conversión de unidad</h3>
      <p className="mt-1 text-sm text-slate-500">SACE no propone ni inventa factores. Registre únicamente un factor respaldado por la fuente indicada.</p>
      <div className="mt-4 grid gap-3 md:grid-cols-4">
        <select className="field md:col-span-2" value={form.materialId} onChange={event => setForm({ ...form, materialId: event.target.value })}>
          <option value="">Material</option>{materials.map(x => <option key={x.id} value={x.id}>{x.materialCode} · {x.description}</option>)}
        </select>
        <input className="field" placeholder="Unidad origen" value={form.fromUnit} onChange={event => setForm({ ...form, fromUnit: event.target.value })}/>
        <input className="field" placeholder="Unidad destino" value={form.toUnit} onChange={event => setForm({ ...form, toUnit: event.target.value })}/>
        <input className="field" type="number" min="0" step="0.000001" placeholder="Factor" value={form.factor} onChange={event => setForm({ ...form, factor: event.target.value })}/>
        <input className="field" type="date" value={form.effectiveFrom} onChange={event => setForm({ ...form, effectiveFrom: event.target.value })}/>
        <input className="field" type="date" value={form.effectiveTo} onChange={event => setForm({ ...form, effectiveTo: event.target.value })}/>
        <input className="field" placeholder="Fuente del factor" value={form.source} onChange={event => setForm({ ...form, source: event.target.value })}/>
        <label className="flex items-center gap-2 text-sm"><input type="checkbox" checked={form.isVerified} onChange={event => setForm({ ...form, isVerified: event.target.checked })}/> Factor verificado</label>
        <button className="btn-primary" disabled={busy || !form.materialId || !form.fromUnit || !form.toUnit || !form.factor || !form.source} onClick={createConversion}>{busy ? <Spinner/> : <Plus size={17}/>}Registrar</button>
      </div>
    </section>
    <section className="table-wrap mb-6">
      <div className="border-b border-slate-200 p-5"><h3 className="font-bold">Conversiones registradas</h3></div>
      <table className="data-table"><thead><tr><th>Material</th><th>Origen</th><th>Destino</th><th>Factor</th><th>Vigencia</th><th>Estado</th><th>Fuente</th></tr></thead>
        <tbody>{conversions.length ? conversions.map(x => <tr key={x.id}><td>{materialName(x.materialId)}</td><td>{x.fromUnit}</td><td>{x.toUnit}</td><td>{number(x.factor)}</td><td>{fmtDate(x.effectiveFrom)} – {fmtDate(x.effectiveTo)}</td><td><Badge value={x.isVerified ? 'VERIFIED' : 'STAGED'}/></td><td>{x.source}</td></tr>) : <tr><td colSpan={7}><Empty text="No hay conversiones registradas. Los descargos con unidades distintas requerirán revisión."/></td></tr>}</tbody>
      </table>
    </section>
    <section className="table-wrap">
      <div className="border-b border-slate-200 p-5"><h3 className="font-bold">Reglas de temporalidad</h3><p className="mt-1 text-sm text-slate-500">Las reglas VERIFIED no pueden editarse directamente desde esta pantalla.</p></div>
      <table className="data-table"><thead><tr><th>Código / versión</th><th>Nombre</th><th>Clasificación</th><th>Tipo</th><th>Duración</th><th>Vigencia</th><th>Estado</th><th>Fuente normativa</th></tr></thead>
        <tbody>{rules.length ? rules.map(x => <tr key={x.id}><td><b>{x.code}</b><small className="block text-slate-500">v{x.version}</small></td><td>{x.name}</td><td>{human(x.inventoryClassification)}</td><td>{human(x.durationType)}</td><td>{x.durationValue ? `${x.durationValue} ${human(x.durationUnit)}` : human(x.durationType)}</td><td>{fmtDate(x.effectiveFrom)} – {fmtDate(x.effectiveTo)}</td><td><Badge value={x.status}/></td><td>{x.officialSourceCitation?.officialLegalSourceVersion?.officialLegalSource ? <a className="font-semibold text-teal-700 underline" href={x.officialSourceCitation.officialUrl || x.officialSourceCitation.officialLegalSourceVersion.officialUrl} target="_blank" rel="noreferrer">{x.officialSourceCitation.citationLabel}</a> : 'Sin fuente visible'}</td></tr>) : <tr><td colSpan={8}><Empty text="No hay reglas de temporalidad configuradas."/></td></tr>}</tbody>
      </table>
    </section>
  </>
}

export function ImmexMaterialUsage() {
  const [programs, setPrograms] = useState<any[]>([])
  const [materials, setMaterials] = useState<any[]>([])
  const [products, setProducts] = useState<any[]>([])
  const [data, setData] = useState<any>()
  const [programId, setProgramId] = useState('')
  const [page, setPage] = useState(1)
  const [showForm, setShowForm] = useState(false)
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const [form, setForm] = useState({ immexProgramId: '', productId: '', materialId: '', periodStart: today(), periodEnd: today(), quantityUsed: '', lossQuantity: '', wasteQuantity: '', unit: 'PZA', sourceType: 'MANUAL' })

  const load = useCallback(async () => {
    setError('')
    try {
      const [programResponse, materialResponse, productResponse, usageResponse] = await Promise.all([
        api.get('/immex/programs'),
        api.get('/immex/materials', { params: { page: 1, pageSize: 100, programId: programId || undefined } }),
        api.get('/immex/products', { params: { page: 1, pageSize: 100, programId: programId || undefined } }),
        api.get('/immex/material-usage', { params: { page, pageSize: 20, programId: programId || undefined } })
      ])
      setPrograms(programResponse.data)
      setMaterials(materialResponse.data.items)
      setProducts(productResponse.data.items)
      setData(usageResponse.data)
    } catch (reason) {
      setError(messageOf(reason))
    }
  }, [page, programId])

  useEffect(() => { void load() }, [load])

  async function save() {
    setBusy(true)
    setError('')
    try {
      await api.post('/immex/material-usage', {
        ...form,
        productId: form.productId || null,
        quantityUsed: Number(form.quantityUsed),
        lossQuantity: Number(form.lossQuantity || 0),
        wasteQuantity: Number(form.wasteQuantity || 0),
        periodStart: new Date(`${form.periodStart}T12:00:00Z`).toISOString(),
        periodEnd: new Date(`${form.periodEnd}T12:00:00Z`).toISOString(),
        sourceDocumentId: null,
        sourceFieldId: null
      })
      setShowForm(false)
      setForm({ ...form, productId: '', materialId: '', quantityUsed: '', lossQuantity: '', wasteQuantity: '' })
      await load()
    } catch (reason) {
      setError(messageOf(reason))
    } finally {
      setBusy(false)
    }
  }

  return <>
    <PageHeader eyebrow="Transformación IMMEX" title="Consumos de materiales" description="Uso, merma y desperdicio se registran como cantidades separadas; ninguno se infiere automáticamente." action={<button className="btn-primary" onClick={() => setShowForm(!showForm)}><Plus size={17}/>Registrar consumo</button>}/>
    <ErrorNotice message={error}/>
    <div className="card mb-5 p-4"><select className="field min-w-72" value={programId} onChange={event => { setProgramId(event.target.value); setPage(1) }}><option value="">Todos los programas</option>{programs.map(x => <option key={x.id} value={x.id}>{x.programNumber}</option>)}</select></div>
    {showForm && <section className="card mb-5 p-5"><div className="grid gap-3 md:grid-cols-4">
      <select className="field" value={form.immexProgramId} onChange={event => setForm({ ...form, immexProgramId: event.target.value, materialId: '', productId: '' })}><option value="">Programa IMMEX</option>{programs.map(x => <option key={x.id} value={x.id}>{x.programNumber}</option>)}</select>
      <select className="field" value={form.materialId} onChange={event => setForm({ ...form, materialId: event.target.value })}><option value="">Material</option>{materials.filter(x => !form.immexProgramId || x.immexProgramId === form.immexProgramId).map(x => <option key={x.id} value={x.id}>{x.materialCode}</option>)}</select>
      <select className="field" value={form.productId} onChange={event => setForm({ ...form, productId: event.target.value })}><option value="">Producto (opcional)</option>{products.filter(x => !form.immexProgramId || x.immexProgramId === form.immexProgramId).map(x => <option key={x.id} value={x.id}>{x.productCode}</option>)}</select>
      <select className="field" value={form.sourceType} onChange={event => setForm({ ...form, sourceType: event.target.value })}>{['MANUAL', 'OPERATION_IMPORT', 'OFFICIAL_SOURCE'].map(x => <option key={x}>{x}</option>)}</select>
      <label className="text-xs text-slate-500">Periodo inicio<input className="field mt-1 w-full" type="date" value={form.periodStart} onChange={event => setForm({ ...form, periodStart: event.target.value })}/></label>
      <label className="text-xs text-slate-500">Periodo fin<input className="field mt-1 w-full" type="date" value={form.periodEnd} onChange={event => setForm({ ...form, periodEnd: event.target.value })}/></label>
      <input className="field" type="number" min="0" step="0.000001" placeholder="Cantidad utilizada" value={form.quantityUsed} onChange={event => setForm({ ...form, quantityUsed: event.target.value })}/>
      <input className="field" type="number" min="0" step="0.000001" placeholder="Merma" value={form.lossQuantity} onChange={event => setForm({ ...form, lossQuantity: event.target.value })}/>
      <input className="field" type="number" min="0" step="0.000001" placeholder="Desperdicio" value={form.wasteQuantity} onChange={event => setForm({ ...form, wasteQuantity: event.target.value })}/>
      <input className="field" placeholder="Unidad" value={form.unit} onChange={event => setForm({ ...form, unit: event.target.value })}/>
      <button className="btn-primary" disabled={busy || !form.immexProgramId || !form.materialId || !form.quantityUsed} onClick={save}>{busy ? <Spinner/> : <CheckCircle2 size={17}/>}Guardar consumo</button>
    </div></section>}
    <section className="table-wrap"><table className="data-table"><thead><tr><th>Periodo</th><th>Material</th><th>Producto</th><th>Utilizado</th><th>Merma</th><th>Desperdicio</th><th>Unidad</th><th>Fuente</th></tr></thead>
      <tbody>{data?.items?.length ? data.items.map((x: any) => <tr key={x.id}><td>{fmtDate(x.periodStart)} – {fmtDate(x.periodEnd)}</td><td>{x.material?.materialCode}</td><td>{x.product?.productCode || '—'}</td><td>{number(x.quantityUsed)}</td><td>{number(x.lossQuantity)}</td><td>{number(x.wasteQuantity)}</td><td>{x.unit}</td><td>{human(x.sourceType)}</td></tr>) : <tr><td colSpan={8}><Empty text="No hay consumos registrados para los filtros seleccionados."/></td></tr>}</tbody>
    </table><PageNav data={data} onPage={setPage}/></section>
  </>
}

export function ImmexIssues() {
  const [programs, setPrograms] = useState<any[]>([])
  const [data, setData] = useState<any>()
  const [programId, setProgramId] = useState('')
  const [status, setStatus] = useState('')
  const [page, setPage] = useState(1)
  const [editing, setEditing] = useState<any>()
  const [nextStatus, setNextStatus] = useState('IN_REVIEW')
  const [resolution, setResolution] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const load = useCallback(async () => {
    setError('')
    try {
      const [programResponse, issueResponse] = await Promise.all([
        api.get('/immex/programs'),
        api.get('/immex/issues', { params: { page, pageSize: 20, programId: programId || undefined, status: status || undefined } })
      ])
      setPrograms(programResponse.data)
      setData(issueResponse.data)
    } catch (reason) {
      setError(messageOf(reason))
    }
  }, [page, programId, status])

  useEffect(() => { void load() }, [load])

  function start(issue: any) {
    setEditing(issue)
    setNextStatus(issue.status === 'OPEN' ? 'IN_REVIEW' : 'RESOLVED')
    setResolution(issue.resolution || '')
  }

  async function update() {
    if (!editing) return
    setBusy(true)
    setError('')
    try {
      await api.patch(`/immex/issues/${editing.id}`, null, { params: { status: nextStatus, resolution: resolution || undefined } })
      setEditing(undefined)
      setResolution('')
      await load()
    } catch (reason) {
      setError(messageOf(reason))
    } finally {
      setBusy(false)
    }
  }

  const closing = nextStatus === 'RESOLVED' || nextStatus === 'DISMISSED'

  return <>
    <PageHeader eyebrow="Conciliación operativa" title="Issues de inventario" description="La resolución es operativa y trazable; no crea hallazgos ni conclusiones del motor LEGAL."/>
    <ErrorNotice message={error}/>
    <div className="card mb-5 grid gap-3 p-4 md:grid-cols-2">
      <select className="field" value={programId} onChange={event => { setProgramId(event.target.value); setPage(1) }}><option value="">Todos los programas</option>{programs.map(x => <option key={x.id} value={x.id}>{x.programNumber}</option>)}</select>
      <select className="field" value={status} onChange={event => { setStatus(event.target.value); setPage(1) }}><option value="">Todos los estados</option>{['OPEN', 'IN_REVIEW', 'RESOLVED', 'DISMISSED'].map(x => <option key={x}>{x}</option>)}</select>
    </div>
    <section className="table-wrap"><table className="data-table"><thead><tr><th>Código</th><th>Severidad</th><th>Estado</th><th>Descripción</th><th>Entrada / salida</th><th>Detectado</th><th>Resolución</th><th/></tr></thead>
      <tbody>{data?.items?.length ? data.items.map((x: any) => <tr key={x.id}><td><b>{x.issueCode}</b></td><td><Badge value={x.severity}/></td><td><Badge value={x.status}/></td><td>{x.description}</td><td><small>Entry: {x.inventoryEntryId || '—'}</small><small className="block">Exit: {x.inventoryExitId || '—'}</small></td><td>{fmtDateTime(x.detectedAt)}</td><td>{x.resolution || '—'}</td><td>{x.status === 'RESOLVED' || x.status === 'DISMISSED' ? null : <button className="btn-secondary" onClick={() => start(x)}>Gestionar</button>}</td></tr>) : <tr><td colSpan={8}><Empty text="No hay issues para los filtros seleccionados."/></td></tr>}</tbody>
    </table><PageNav data={data} onPage={setPage}/></section>
    {editing && <div className="fixed inset-0 z-50 grid place-items-center bg-slate-950/50 p-4"><div className="w-full max-w-xl rounded-2xl bg-white p-6 shadow-2xl"><h3 className="text-xl font-bold">Gestionar {editing.issueCode}</h3><p className="mt-1 text-sm text-slate-500">{editing.description}</p><div className="mt-5 space-y-3">
      <select className="field w-full" value={nextStatus} onChange={event => setNextStatus(event.target.value)}>{editing.status === 'OPEN' && <option value="IN_REVIEW">IN_REVIEW</option>}<option value="RESOLVED" disabled={editing.status === 'OPEN'}>RESOLVED</option><option value="DISMISSED">DISMISSED</option></select>
      <textarea className="field min-h-28 w-full" placeholder={closing ? 'Resolution obligatoria para cerrar' : 'Nota operativa opcional'} value={resolution} onChange={event => setResolution(event.target.value)}/>
    </div><div className="mt-5 flex justify-end gap-3"><button className="btn-secondary" onClick={() => setEditing(undefined)}>Cancelar</button><button className="btn-primary" disabled={busy || (closing && !resolution.trim())} onClick={update}>{busy ? <Spinner/> : <CheckCircle2 size={17}/>}Actualizar</button></div></div></div>}
  </>
}
