import React, {Component} from 'react';
import { Table } from 'react-bootstrap';
import moment from 'moment';
import {If, Then, ElseIf, Else} from 'react-if-elseif-else-render';

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
                            <th>Moon point</th>
                            <th>Jupiter point</th>
                            <th>Neptune point</th>
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
                                <If condition={person.venusPos > 6}>
                                    <Then>
                                        <td style={{backgroundColor:"aquamarine"}}>{person.venusPos}</td>
                                    </Then>
                                    <Else>
                                        <td>{person.venusPos}</td>
                                    </Else>
                                </If>                                
                                <If condition={person.marsPos < 15}>
                                    <Then>
                                        <td style={{backgroundColor:"turquoise"}}>{moment(new Date()).add(person.marsPos, 'minute').format('HH:mm')}</td>
                                    </Then>
                                    <Else>
                                        <td>{moment(new Date()).add(person.marsPos, 'minute').format('HH:mm')}</td>
                                    </Else>
                                </If>
                                <If condition={person.mercuryPos < 15}>
                                    <Then>
                                        <td style={{backgroundColor:"turquoise"}}>{moment(new Date()).add(person.mercuryPos, 'minute').format('HH:mm')}</td>
                                    </Then>
                                    <Else>
                                        <td>{moment(new Date()).add(person.mercuryPos, 'minute').format('HH:mm')}</td>
                                    </Else>
                                </If>
                                <If condition={person.uranusPos < 15}>
                                    <Then>
                                        <td style={{backgroundColor:"turquoise"}}>{moment(new Date()).add(person.uranusPos, 'minute').format('HH:mm')}</td>
                                    </Then>
                                    <Else>
                                        <td>{moment(new Date()).add(person.uranusPos, 'minute').format('HH:mm')}</td>
                                    </Else>
                                </If>
                                <If condition={person.jupiterPos < 15}>
                                    <Then>
                                        <td style={{backgroundColor:"turquoise"}}>{moment(new Date()).add(person.jupiterPos, 'minute').format('HH:mm')}</td>
                                    </Then>
                                    <Else>
                                        <td>{moment(new Date()).add(person.jupiterPos, 'minute').format('HH:mm')}</td>
                                    </Else>
                                </If>
                                <td>{person.neptunePos}</td>
                            </tr>)}
                    </tbody>
                </Table>
            </div>
        )
    }
}