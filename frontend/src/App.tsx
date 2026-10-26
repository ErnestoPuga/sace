import { Navigate, Route, Routes } from 'react-router-dom'
import { Layout } from './components'
import { Login, Dashboard } from './pages/CorePages'
import { Operations, NewOperation, EditOperation, OperationDetail } from './pages/OperationPages'
import { Findings, DailyAudits, MonthlyAudits, NormativeRules, RegulatoryRequirements, LegalSources, History } from './pages/AdminPages'
import { BatchAuditsV2 as BatchAudits, RiskCenterV2 as RiskCenter, ReviewQueue } from './pages/RiskPages'

function Protected(){return localStorage.getItem('sace_token')?<Layout/>:<Navigate to="/login" replace/>}
export default function App(){return <Routes><Route path="/login" element={<Login/>}/><Route element={<Protected/>}><Route index element={<Dashboard/>}/><Route path="operations" element={<Operations/>}/><Route path="operations/new" element={<NewOperation/>}/><Route path="operations/:id/edit" element={<EditOperation/>}/><Route path="operations/:id" element={<OperationDetail/>}/><Route path="batch-audits" element={<BatchAudits/>}/><Route path="risk" element={<RiskCenter/>}/><Route path="review-queue" element={<ReviewQueue/>}/><Route path="findings" element={<Findings/>}/><Route path="audits/daily" element={<DailyAudits/>}/><Route path="audits/monthly" element={<MonthlyAudits/>}/><Route path="normative-rules" element={<NormativeRules/>}/><Route path="legal-sources" element={<LegalSources/>}/><Route path="regulatory-requirements" element={<RegulatoryRequirements/>}/><Route path="history" element={<History/>}/></Route><Route path="*" element={<Navigate to="/" replace/>}/></Routes>}
