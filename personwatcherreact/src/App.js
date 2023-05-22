import './App.css';
import './fonts/astro.ttf';

import React, {useEffect, useState} from 'react';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import {Home} from './Home';
import {Person} from './Person';
import {Ranking} from './Ranking';
import {Navigation} from './Navigation';

function App() {
  const MINUTE_MS = 60000;
  const [header, setHeader] = useState(); 
  const getApiData = async () => {
    const response = await fetch(
      process.env.REACT_APP_API+'Person/Header'
    ).then((response) => response.json());
    // update the state
    setHeader(response.data);
  };

  useEffect(() => {const interval = setInterval(() => {
      getApiData();
   }, MINUTE_MS);

    return () => clearInterval(interval);
    
  }, []);

  return (
    <BrowserRouter>
    <div className="container">
     <h3 className="m-3 d-flex justify-content-center astro">
       {header}
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
