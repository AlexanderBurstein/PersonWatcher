import React, {Component} from 'react';
import { NavLink} from 'react-router-dom';
import { Navbar, Nav, Form, FormControl, Button } from 'react-bootstrap';

export class Navigation extends Component {
    render() {
        return (
            <Navbar bg="light" expand="lg">
                <Navbar.Toggle aria-controls="basic-navbar-nav"/>
                <Navbar.Collapse id="basic-navbar-nav">
                    <Nav>
                        <NavLink className="d-inline p-2 bg-light text-blue" to="/person" >
                            Person
                        </NavLink>
                        <NavLink className="d-inline p-2 bg-light text-blue" to="/ranking" >
                            Ranking
                        </NavLink>
                    </Nav>
                </Navbar.Collapse>
            </Navbar>
        )
    }
}