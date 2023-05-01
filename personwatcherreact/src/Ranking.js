import React, {Component} from 'react';
import { Table } from 'react-bootstrap';
import moment from 'moment';

export class Ranking extends Component {
    constructor(props) {
        super(props);
        this.state={persons:[], AppModalShow:false};
    }

    refreshList(){
        fetch(process.env.REACT_APP_API+'Person/Rank')
        .then(response=>response.json())
        .then(data=>{
            this.setState({persons:data});
        })
    }

    componentDidMount() {
        this.refreshList();
    }

    render() {
        const {persons, personid}=this.state;
        let addModalClose=()=>this.setState({addModalShow:false});
        let editModalClose=()=>this.setState({editModalShow:false});
           return (
            <div >
                <Table className="mt-4" striped bordered hover size="sm">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Next Start</th>
                            <th>Pluses</th>
                            <th>Minuses</th>
                            <th>Sign Matches</th>
                            <th>Young</th>
                            <th>Experienced</th>
                            <th>Next Action</th>
                            <th>Impulsivity</th>
                        </tr>
                    </thead>
                    <tbody>
                        {persons.map(person=>
                            <tr key={person.personId}>
                                <td>{person.name}</td>
                                <td>{moment(person.nextStart).calendar()}</td>
                                <td>{person.sunPos}</td>
                                <td>{person.moonPos}</td>
                                <td>{person.venusPos}</td>
                                <td>{person.marsPos}</td>
                                <td>{person.saturnPos}</td>
                                <td>{person.jupiterPos}</td>
                                <td>{person.neptunePos}</td>
                            </tr>)}
                    </tbody>
                </Table>
            </div>
        )
    }
}