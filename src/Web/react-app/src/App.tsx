import { BrowserRouter, Route, Routes } from 'react-router-dom'
import './App.css'
import Main from './components/Main'
import GuardedRoutes from './routes/GuardedRoutes'
import RouteAuthGuard from './routes/RouteAuthGuard'

function App() {
  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route path='/' element={<Main />}>
            {GuardedRoutes.map((route, index) => {
              return <RouteAuthGuard key={index} path={route.path} component={route.component} />
            })}
          </Route>
        </Routes>
      </BrowserRouter>
    </>
  )
}
export default App