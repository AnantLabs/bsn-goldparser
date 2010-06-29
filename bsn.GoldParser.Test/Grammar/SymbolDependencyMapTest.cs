﻿using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace bsn.GoldParser.Grammar {
	[TestFixture]
	public class SymbolDependencyMapTest: AssertionHelper {
		[Test]
		public void Create() {
			new SymbolDependencyMap();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddNullSymbol() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			SymbolDependencyMap map = new SymbolDependencyMap();
			map.AddDependecy(null, grammar.GetSymbol(0));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddNullDependency() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			SymbolDependencyMap map = new SymbolDependencyMap();
			map.AddDependecy(grammar.GetSymbol(0), null);
		}

		[Test]
		public void AddSelfDependency() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbol = grammar.GetSymbol(0);
			SymbolDependencyMap map = new SymbolDependencyMap();
			map.AddDependecy(symbol, symbol);
		}

		[Test]
		public void AddCircularDependency() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbolX = grammar.GetSymbol(0);
			Symbol symbolY = grammar.GetSymbol(1);
			Symbol symbolZ = grammar.GetSymbol(2);
			SymbolDependencyMap map = new SymbolDependencyMap();
			map.AddDependecy(symbolX, symbolY);
			map.AddDependecy(symbolY, symbolZ);
			map.AddDependecy(symbolZ, symbolX);
		}

		[Test]
		public void AddSimpleDependency() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbolX = grammar.GetSymbol(0);
			Symbol symbolY = grammar.GetSymbol(1);
			SymbolDependencyMap map = new SymbolDependencyMap();
			map.AddDependecy(symbolX, symbolY);
			Expect(map.DependsOn(symbolX, symbolY), True);
			Expect(map.DependsOn(symbolY, symbolX), False);
	}

		[Test]
		public void AddNestedDependency() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbolX = grammar.GetSymbol(0);
			Symbol symbolY = grammar.GetSymbol(1);
			Symbol symbolZ = grammar.GetSymbol(2);
			SymbolDependencyMap map = new SymbolDependencyMap();
			map.AddDependecy(symbolX, symbolY);
			map.AddDependecy(symbolY, symbolZ);
			Expect(map.DependsOn(symbolX, symbolZ), True);
			Expect(map.DependsOn(symbolY, symbolX), False);
			Expect(map.DependsOn(symbolZ, symbolX), False);
		}

		[Test]
		public void CompareNested() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbolX = grammar.GetSymbol(0);
			Symbol symbolY = grammar.GetSymbol(1);
			Symbol symbolZ = grammar.GetSymbol(2);
			Symbol symbolA = grammar.GetSymbol(3);
			SymbolDependencyMap map = new SymbolDependencyMap();
			map.AddDependecy(symbolX, symbolY);
			map.AddDependecy(symbolY, symbolZ);
			Expect(map.Compare(symbolX, symbolX), EqualTo(0));
			Expect(map.Compare(symbolX, symbolY), GreaterThan(0));
			Expect(map.Compare(symbolY, symbolX), LessThan(0));
			Expect(map.Compare(symbolX, symbolZ), GreaterThan(0));
			Expect(map.Compare(symbolZ, symbolX), LessThan(0));
			Expect(map.Compare(symbolY, symbolZ), GreaterThan(0));
			Expect(map.Compare(symbolZ, symbolY), LessThan(0));
			Expect(map.Compare(symbolA, symbolX), GreaterThan(0));
			Expect(map.Compare(symbolA, symbolY), GreaterThan(0));
			Expect(map.Compare(symbolA, symbolZ), GreaterThan(0));
			map.AddDependecy(symbolY, symbolA);
			Expect(map.Compare(symbolA, symbolX), LessThan(0));
			Expect(map.Compare(symbolA, symbolY), LessThan(0));
			Expect(map.Compare(symbolA, symbolZ), GreaterThan(0));
		}

		[Test]
		public void GetDependencies() {
			CompiledGrammar grammar = CompiledGrammarTest.LoadTestGrammar();
			Symbol symbolX = grammar.GetSymbol(0);
			Symbol symbolY = grammar.GetSymbol(1);
			Symbol symbolZ = grammar.GetSymbol(2);
			SymbolDependencyMap map = new SymbolDependencyMap();
			map.AddDependecy(symbolX, symbolY);
			map.AddDependecy(symbolY, symbolZ);
			List<Symbol> dependencies = new List<Symbol>(2);
			dependencies.Add(symbolY);
			dependencies.Add(symbolZ);
			using (IEnumerator<Symbol> enumerator = map.GetDependencies(symbolX).GetEnumerator()) {
				Expect(enumerator.MoveNext(), True);
				Expect(dependencies.Remove(enumerator.Current), True);
				Expect(enumerator.MoveNext(), True);
				Expect(dependencies.Remove(enumerator.Current), True);
				Expect(enumerator.MoveNext(), False);
			}
			Expect(dependencies.Count, EqualTo(0));
		}
	}
}