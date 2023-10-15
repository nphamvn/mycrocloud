import { BrowserRouter, Route, Routes } from 'react-router-dom'
import './App.css'
import Home from './components/Home'
import RouteList from './components/routes/RouteList'
import AppList from './components/apps/AppList'
import AppCreate from './components/apps/AppCreate'
import { AuthContext } from './context/AuthContext'
import { useAuth } from './hooks/useAuth'
import Header from './components/Header'
import { useEffect } from 'react'

function App() {
  const { user, login, logout, setUser } = useAuth();

  useEffect(() => {
    login({
      id: 'nampham',
      accessToken: ''
    })
  }, [])

  return (
    <>
      <AuthContext.Provider value={{ user, setUser }} >
        <Header />
        <BrowserRouter>
          <Routes>
            <Route path='/' Component={Home} />
            <Route path='/apps' element={<AppList />} />
            <Route path='/apps/new' element={<AppCreate />} />
            <Route path='/apps/:appId/routes' element={<RouteList />} />
          </Routes>
        </BrowserRouter>
      </AuthContext.Provider>
    </>
  )
}
export default App