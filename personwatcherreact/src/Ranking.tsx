import React, {Component} from 'react';
import { Table } from 'react-bootstrap';
import moment from 'moment';
import {PersonData} from './PersonData';

type rankingTypes = {
    [key: string]: any
  };
type defaultRankingProps = {
    persons:PersonData[], 
  };

export class Ranking extends Component<rankingTypes, defaultRankingProps> {
    constructor(props:rankingTypes){
        super(props);
        this.state={persons:[] as PersonData[]};
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
           return (
            <div >
                <Table className="mt-4" striped bordered hover size="sm">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Next Start</th>
                            <th>Pluses</th>
                            <th>Minuses</th>
                            <th>Astro info</th>
                            <th>Next Action</th>
                            <th>Impulsivity</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.state.persons.map(person=>
                            <tr key={person.personId}>
                                <td>{person.name}</td>
                                <td>{moment(person.nextStart).calendar()}</td>
                                <td>{person.sunPos}</td>
                                <td>{person.moonPos}</td>
                                <td className="astro" 
                                    style={
                                        person.extraInfo.startsWith("1") ? {backgroundColor:"lightgreen"} :
                                        person.extraInfo.startsWith("2") ? {backgroundColor:"lightpink"} :
                                        person.extraInfo.startsWith("3") ? {backgroundColor:"aqua"} :
                                        person.extraInfo.startsWith("4") ? {backgroundColor:"plum"} :
                                        {}}>{person.extraInfo.substring(1)}</td>
                                <td style={
                                    person.jupiterPos < 15 ? {backgroundColor:"turquoise"} :
                                    {}}>{moment(new Date()).add(person.jupiterPos, 'minute').format('HH:mm')}</td>
                                <td>{person.neptunePos}</td>
                            </tr>)}
                    </tbody>
                </Table>
            </div>
        )
    }
}