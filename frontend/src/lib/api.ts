import axios from 'axios'
export const api = axios.create({ baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5092/api' })
api.interceptors.request.use(config => { const token = localStorage.getItem('sace_token'); if (token) config.headers.Authorization = `Bearer ${token}`; return config })
api.interceptors.response.use(r => r, error => { if (error.response?.status === 401 && !location.pathname.includes('/login')) { localStorage.removeItem('sace_token'); location.href='/login' } return Promise.reject(error) })
export const messageOf = (error: any) => error?.response?.data?.message || 'No fue posible completar la operación.'
