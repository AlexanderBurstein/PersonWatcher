import React, {Component} from 'react';
import { ButtonToolbar, Button, Table } from 'react-bootstrap';
import {AddPersonModal} from './AddPersonModal';
import { EditPersonModal } from './EditPersonModal';
import SearchBar from './SearchBar.js';

import moment from 'moment';
const SEARCH_PERSON_URI = process.env.REACT_APP_API+'Person';

export class Person extends Component {

    constructor(props) {
        super(props);
        this.state={persons:[], isLoading:false, AppModalShow:false, EditModalShow:false};
    }

    _handleSearch = query => {
        this.setState({ isLoading: true });
        this.makeAndHandleRequest(query).then(({ opts }) => {
          this.setState(
            {
              isLoading: false,
              keyword: query,
              persons: opts
            }
          );
        });
      };
    
      _birthdaySearch = dateStr => {
        this.setState({ isLoading: true });
        this.makeAndHandleRequest("", dateStr).then(({ opts }) => {
          this.setState(
            {
              isLoading: false,
              persons: opts
            }
          );
        });
      };

       makeAndHandleRequest(query, dateStr, page = 1) {
        return fetch(`${SEARCH_PERSON_URI}?searchStr=${query}&dateStr=${dateStr}`)
          .then(resp => resp.json())
          .then(data => {
            const total_count=data.length;
            const opts = data;
            return { opts, total_count };
          });
      }

    render() {
        const {persons, personid, personname, eventtype, birthdate, nextstart, keyword}=this.state;
        let addModalClose=dateStr=>{this._birthdaySearch(dateStr);this.setState({addModalShow:false, keyword:''})};
        let editModalClose=dateStr=>{this._birthdaySearch(dateStr);this.setState({editModalShow:false, keyword:''})};
            return (
            <div >
                <SearchBar keyword={keyword} onChange={(e) => {this._handleSearch(e);}}/>
                <Table className="mt-4" striped bordered hover size="sm">
                    <thead>
                        <tr>
                            <th>EventType</th>
                            <th>Name</th>
                            <th>Next Start</th>
                            <th>Options</th>
                        </tr>
                    </thead>
                    <tbody>
                        {persons.map(person=>
                            <tr key={person.personId}>
                                <td>{person.eventType}</td>
                                <td>{person.name}</td>
                                <td>{moment(person.nextStart).calendar()}</td>
                                <td>
                                    <ButtonToolbar>
                                        <Button className="mr-2" variant="info"
                                        onClick={()=>this.setState({editModalShow:true,
                                            personid:person.personId,
                                            personname:person.name,
                                            eventype:person.eventType,
                                            eventpredictability:person.eventPredictability,
                                            birthdate:person.birthdate,
                                            nextstart:person.nextStart})}>
                                                Edit
                                            </Button>

                                    </ButtonToolbar>
                                </td>
                            </tr>)}
                    </tbody>
                </Table>

                <ButtonToolbar>
                    <Button variant='primary'
                    onClick={()=>this.setState({addModalShow:true})}>
                        Add Person
                    </Button>

                    <AddPersonModal show={this.state.addModalShow}
                    onHide={addModalClose}
                    personid="0"
                    personname=""
                    eventtype="0"/>
                    <EditPersonModal show={this.state.editModalShow}
                            onHide={editModalClose}
                            personid={personid}
                            personname={personname}
                            eventtype={eventtype}
                            eventpredictabilty={eventpredictability}
                            birthdate={birthdate}
                            nextstart={nextstart}/>
                </ButtonToolbar>
            </div>
        )
    }
}