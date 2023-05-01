import './App.css';

import { BrowserRouter, Routes, Route } from "react-router-dom";
import {Home} from './Home';
import {Person} from './Person';
import {Ranking} from './Ranking';
import {Navigation} from './Navigation';

function App() {
  return (
    <BrowserRouter>
    <div className="container">
     <h3 className="m-3 d-flex justify-content-center">
       Person Watcher
     </h3>

     <Navigation/>

     <Routes>
      <Route path='/' element={<Home/>}/>
       <Route path='/person' element={<Person/>}/>
       <Route path='/ranking' element={<Ranking/>}/>
     </Routes>
    </div>
    </BrowserRouter>
  );
}
export default App;
