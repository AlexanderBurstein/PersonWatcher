import React, {Component} from 'react';
import { ButtonToolbar, Button, Table } from 'react-bootstrap';
import {AddPersonModal} from './AddPersonModal';
import { EditPersonModal } from './EditPersonModal';
import SearchBar from './SearchBar';
import {PersonData} from './PersonData';
import moment from 'moment';
const SEARCH_PERSON_URI = process.env.REACT_APP_API+'Person';

type personTypes = {
  [key: string]: any
};
type defaultPersonProps = {
  persons:PersonData[], 
  selected:PersonData,
  isLoading:boolean, 
  AppModalShow:boolean, 
  EditModalShow:boolean,
  keyword: string
};

export class Person extends Component<personTypes, defaultPersonProps> {
  constructor(props:personTypes){
        super(props);
        this.state={
          persons:[] as PersonData[], 
          selected: {} as PersonData,
          isLoading:false, 
          AppModalShow:false, 
          EditModalShow:false,
          keyword:''
        };
    }

    _handleSearch = (query: string) => {
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
    
      _birthdaySearch = (dateStr:string) => {
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

       makeAndHandleRequest(query: string, dateStr: string = '', page = 1) {
        return fetch(`${SEARCH_PERSON_URI}?searchStr=${query}&dateStr=${dateStr}`)
          .then(resp => resp.json())
          .then(data => {
            const total_count=data.length;
            const opts = data;
            return { opts, total_count };
          });
      }

    render() {
        let addModalClose=(dateStr:string)=>{this._birthdaySearch(dateStr);this.setState({AppModalShow:false, keyword:''})};
        let editModalClose=(dateStr:string)=>{this._birthdaySearch(dateStr);this.setState({EditModalShow:false, keyword:''})};
            return (
            <div >
                <SearchBar keyword={this.state.keyword} searchDeals={(e:any) => {this._handleSearch(e);}}/>
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
                        {this.state.persons.map(person=>
                            <tr key={person.personId}>
                                <td>{person.eventType}</td>
                                <td>{person.eventPredictability} {person.name}</td>
                                <td>{moment(person.nextStart).calendar()}</td>
                                <td>
                                    <ButtonToolbar>
                                        <Button className="mr-2" variant="info"
                                        onClick={()=>this.setState({
                                            EditModalShow:true,
                                            selected: person
                                            })}>
                                                Edit
                                            </Button>

                                    </ButtonToolbar>
                                </td>
                            </tr>)}
                    </tbody>
                </Table>

                <ButtonToolbar>
                    <Button variant='primary'
                    onClick={()=>this.setState({AppModalShow:true})}>
                        Add Person
                    </Button>

                    <AddPersonModal show={this.state.AppModalShow}
                    onHide={addModalClose}
                    personid="0"
                    personname=""
                    eventtype="0"/>
                    <EditPersonModal show={this.state.EditModalShow}
                            onHide={editModalClose}
                            personid={this.state.selected.personId}
                            personname={this.state.selected.name}
                            eventtype={this.state.selected.eventType}
                            eventpredictabilty={this.state.selected.eventPredictability}
                            birthdate={this.state.selected.birthdate}
                            nextstart={this.state.selected.nextStart}/>
                </ButtonToolbar>
            </div>
        )
    }
}